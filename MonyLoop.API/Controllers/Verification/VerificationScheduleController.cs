using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MonyLoop.Application.DTOs.Verification;
using MonyLoop.Application.ServicesAbstractions.Verification;

namespace MonyLoop.Api.Controllers.Verification
{
    [ApiController]
    [Route("api/verification-schedules")]
    [Authorize]
    public class VerificationScheduleController : ControllerBase
    {
        private readonly IVerificationScheduleService _scheduleService;

        public VerificationScheduleController(IVerificationScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        [HttpPost]
        public async Task<ActionResult<VerificationScheduleResponseDto>> CreateSchedule([FromBody] CreateVerificationScheduleDto dto, CancellationToken ct)
        {
            var result = await _scheduleService.CreateScheduleAsync(dto, ct);
            return CreatedAtAction(nameof(GetScheduleById), new { id = result.VerificationScheduleId }, result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<VerificationScheduleResponseDto>> GetScheduleById([FromRoute] Guid id, CancellationToken ct)
        {
            var schedule = await _scheduleService.GetScheduleByIdAsync(id, ct);
            if (schedule == null)
            {
                return NotFound();
            }

            return Ok(schedule);
        }

        [HttpGet("application/{applicationId:guid}")]
        public async Task<ActionResult<IReadOnlyList<VerificationScheduleResponseDto>>> GetSchedulesByApplication([FromRoute] Guid applicationId, CancellationToken ct)
        {
            var schedules = await _scheduleService.GetSchedulesByApplicationIdAsync(applicationId, ct);
            return Ok(schedules);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateSchedule([FromRoute] Guid id, [FromBody] UpdateVerificationScheduleDto dto, CancellationToken ct)
        {
            try
            {
                await _scheduleService.UpdateScheduleStatusAsync(id, dto, ct);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
