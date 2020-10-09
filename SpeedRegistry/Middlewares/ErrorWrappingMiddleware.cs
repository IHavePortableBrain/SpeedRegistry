using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using SpeedRegistry.Core.Exceptions;
using SpeedRegistry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpeedRegistry.Middlewares
{
    public class ErrorWrappingMiddleware
    {
        private static readonly ActionDescriptor _emptyActionDescriptor = new ActionDescriptor();
        private static readonly RouteData _emptyRouteData = new RouteData();
        private static readonly HashSet<string> _corsHeaderNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            HeaderNames.AccessControlAllowCredentials,
            HeaderNames.AccessControlAllowHeaders,
            HeaderNames.AccessControlAllowMethods,
            HeaderNames.AccessControlAllowOrigin,
            HeaderNames.AccessControlExposeHeaders,
            HeaderNames.AccessControlMaxAge,
        };

        private readonly RequestDelegate _next;
        private readonly IActionResultExecutor<ObjectResult> _executor;
        private readonly ILogger<ErrorWrappingMiddleware> _logger;

        public ErrorWrappingMiddleware(
            RequestDelegate next,
            IActionResultExecutor<ObjectResult> executor,
            ILogger<ErrorWrappingMiddleware> logger)
        {
            _next = next;
            _executor = executor;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());

                if (context.Response.HasStarted)
                {
                    throw;
                }

                var problemDetails = CreateProblemDetails(context.Request.Path, context.TraceIdentifier, ex);

                ClearResponse(context, problemDetails.Status.Value);
                await WriteProblemDetails(context, problemDetails);

                return;
            }
        }

        private Task WriteProblemDetails(HttpContext context, ProblemDetails details)
        {
            var routeData = context.GetRouteData() ?? _emptyRouteData;

            var actionContext = new ActionContext(context, routeData, _emptyActionDescriptor);

            var result = new ObjectResult(details)
            {
                StatusCode = details.Status ?? context.Response.StatusCode,
                DeclaredType = details.GetType(),
            };

            result.ContentTypes.Add("application/problem+json");
            result.ContentTypes.Add("application/problem+xml");

            return _executor.ExecuteAsync(actionContext, result);
        }

        private void ClearResponse(HttpContext context, int statusCode)
        {
            var headers = new HeaderDictionary();

            // Make sure problem responses are never cached.
            headers.Append(HeaderNames.CacheControl, "no-cache, no-store, must-revalidate");
            headers.Append(HeaderNames.Pragma, "no-cache");
            headers.Append(HeaderNames.Expires, "0");

            foreach (var header in context.Response.Headers)
            {
                // Because the CORS middleware adds all the headers early in the pipeline,
                // we want to copy over the existing Access-Control-* headers after resetting the response.
                if (_corsHeaderNames.Contains(header.Key))
                {
                    headers.Add(header);
                }
            }

            context.Response.Clear();
            context.Response.StatusCode = statusCode;

            foreach (var header in headers)
            {
                context.Response.Headers.Add(header);
            }
        }

        private ProblemDetails CreateProblemDetails(string instance, string traceId, Exception exception)
        {
            switch (exception)
            {
                case OutOfServiceException ex:
                    return new ProblemDetails()
                    {
                        Instance = instance,
                        Status = StatusCodes.Status400BadRequest,
                        Title = ex.Message,
                    };

                default:
                    return new UnexpectedProblemDetails()
                    {
                        Instance = instance,
                        Status = StatusCodes.Status500InternalServerError,
                        Title = exception.Message,
                        Timestamp = DateTime.UtcNow,
                        TraceId = traceId,
                        Detail = exception.ToString(),
                    };
            }
        }
    }
}
