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
        [Route("entries")]
        public async Task<IEnumerable<SpeedEntryDto>> GetAllEntriesAsync()
        {
            var result = await _speedControllerService.GetSpeedEntriesAsync();
            return result;
        }

        [HttpPost]
        [Route("entries")]
        public async Task<ActionResult> PostCreateEntryAsync(SpeedEntryDto model)
        {
            await _speedControllerService.CreateSpeedEntryAsync(model);
            return Ok();
        }

        [HttpPost]
        [Route("entries/test")]
        public async Task<ActionResult> PostCreateTestEntriesAsync()
        {
            await _speedControllerService.CreateTestEntriesAsync();
            return Ok();
        }
    }
}
