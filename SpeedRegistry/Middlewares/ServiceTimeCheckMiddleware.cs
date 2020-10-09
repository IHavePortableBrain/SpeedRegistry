using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SpeedRegistry.Core.Exceptions;
using SpeedRegistry.Core.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpeedRegistry.Middlewares
{
    public class ServiceTimeCheckMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserContextMiddleware"/> class.
        /// </summary>
        public ServiceTimeCheckMiddleware(RequestDelegate next, IOptions<AppSettings> options)
        {
            _next = next;
            _appSettings = options.Value;
        }

        public async Task Invoke(HttpContext context)
        {
            if ((DateTime.UtcNow.TimeOfDay < _appSettings.ServiceStartTime 
                || DateTime.UtcNow.TimeOfDay > _appSettings.ServiceEndTime)
                && context.Request.Method == "GET") // todo: this predicate might be changed to role check when roles appear 
            {
                throw new OutOfServiceException($"Service work time is {_appSettings.ServiceStartTime}-{_appSettings.ServiceEndTime}.");
            }

            await _next.Invoke(context);
        }
    }
}
