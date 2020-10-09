using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SpeedRegistry.Business.ControllerServices;
using SpeedRegistry.Business.Dto;
using SpeedRegistry.Core;

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
        public async Task<ActionResult<IEnumerable<SpeedEntryDto>>> GetAllEntriesAsync()
        {
            var result = await _speedControllerService.GetAllEntriesAsync();
            return Ok(result);
        }

        [HttpGet]
        [Route("entries/over-speed")]
        public async Task<ActionResult<IEnumerable<SpeedEntryDto>>> GetOverSpeedEntriesAsync([FromQuery]GetMinMaxSpeedEntryDto dto)
        {
            var result = await _speedControllerService.GetOverSpeedEntriesAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Route("entries/min-max")]
        public async Task<ActionResult<MinMaxSpeedEntryDto>> GetMinMaxSpeedEntriesAsync([FromQuery] ClosedPeriod period)
        {
            var result = await _speedControllerService.GetMinMaxSpeedEntriesAsync(period);
            return Ok(result);
        }

        [HttpPost]
        [Route("entries/{id}")]
        public async Task<ActionResult> PostCreateEntryAsync(SpeedEntryDto dto)
        {
            await _speedControllerService.CreateSpeedEntryAsync(dto);
            return Ok();
        }

        [HttpPost]
        [Route("entries")]
        public async Task<ActionResult> PostCreateEntriesAsync(IEnumerable<SpeedEntryDto> dtos)
        {
            await _speedControllerService.CreateSpeedEntriesAsync(dtos);
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
