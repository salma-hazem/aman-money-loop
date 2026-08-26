using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MonyLoop.Application.DTOs.Verification;
using MonyLoop.Application.ServicesAbstractions.Verification;
using Microsoft.AspNetCore.Authorization;
using MonyLoop.Domain.Entities.UserAuth;

namespace MonyLoop.Api.Controllers.Verification
{
    [ApiController]
    [Route("api/verification-checklists")]
    [Authorize(Roles = $"{ApplicationRole.Admin},{ApplicationRole.Organizer}")]
    public class VerificationChecklistController : ControllerBase
    {
        private readonly IVerificationChecklistService _checklistService;

        public VerificationChecklistController(IVerificationChecklistService checklistService)
        {
            _checklistService = checklistService;
        }

        [HttpPost("submit")]
        public async Task<ActionResult<VerificationChecklistSubmissionResponseDto>> SubmitChecklist([FromBody] CreateVerificationChecklistSubmissionDto dto, CancellationToken ct)
        {
            try
            {
                var result = await _checklistService.SubmitChecklistAsync(dto, ct);
                return CreatedAtAction(nameof(GetSubmissionBySchedule), new { scheduleId = dto.VerificationScheduleId }, result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("schedule/{scheduleId:guid}")]
        public async Task<ActionResult<VerificationChecklistSubmissionResponseDto>> GetSubmissionBySchedule([FromRoute] Guid scheduleId, CancellationToken ct)
        {
            var submission = await _checklistService.GetSubmissionByScheduleIdAsync(scheduleId, ct);
            if (submission == null)
            {
                return NotFound();
            }

            return Ok(submission);
        }

        [HttpGet("schedule/{scheduleId:guid}/consolidated-result")]
        public async Task<ActionResult<VerificationConsolidatedResultDto>> GetConsolidatedResult([FromRoute] Guid scheduleId, CancellationToken ct)
        {
            var result = await _checklistService.GetConsolidatedResultAsync(scheduleId, ct);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}