using System.Collections.Generic;
using System.Threading.Tasks;
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
        public async Task<IEnumerable<SpeedEntryDto>> Get()
        {
            var result = await _speedControllerService.GetSpeedEntriesAsync();
            return result;
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync()
        {
            await _speedControllerService.SaveSomeSpeedEntriesAsync();
            return Ok();
        }
    }
}
