using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SpeedRegistry.Business.ControllerServices;
using SpeedRegistry.Business.Dto;

namespace SpeedRegistry.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SpeedController : ControllerBase
    {
        private readonly ILogger<SpeedController> _logger;
        private readonly SpeedControllerService _speedControllerService;

        public SpeedController(ILogger<SpeedController> logger, SpeedControllerService speedControllerService)
        {
            _logger = logger;
            _speedControllerService = speedControllerService;
        }

        [HttpGet]
        public IEnumerable<SpeedEntryDto> Get()
        {
            return new SpeedEntryDto[] { new SpeedEntryDto() { Speed = 12.2f } };
        }
    }
}
