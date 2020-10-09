using Microsoft.AspNetCore.Mvc;
using System;

namespace SpeedRegistry.Models
{
    public class UnexpectedProblemDetails : ProblemDetails
    {
        public DateTime Timestamp { get; set; }

        public string TraceId { get; set; }
    }
}
