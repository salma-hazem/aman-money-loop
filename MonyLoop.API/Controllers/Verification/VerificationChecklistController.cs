using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonyLoop.Application.DTOs.Verification;
using MonyLoop.Application.ServicesAbstractions.Verification;
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
            // 1. Extract authenticated reviewer identity from JWT claims
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                           ?? User.FindFirstValue("sub");

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var authenticatedUserId))
            {
                return Unauthorized("User identity claim is missing or invalid in authentication token.");
            }

            // 2. Override client-provided ID with the verified authenticated User ID
            dto.SubmittedByUserId = authenticatedUserId;

            try
            {
                var result = await _checklistService.SubmitChecklistAsync(dto, ct);
                return CreatedAtAction(nameof(GetSubmissionBySchedule), new { scheduleId = dto.VerificationScheduleId }, result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("application/{applicationId:guid}/consolidated-result")]
        public async Task<ActionResult<ApplicationVerificationSummaryDto>> GetApplicationConsolidatedSummary([FromRoute] Guid applicationId, CancellationToken ct)
        {
            var result = await _checklistService.GetApplicationConsolidatedSummaryAsync(applicationId, ct);
            if (result == null)
            {
                return NotFound($"No evaluation history found for application {applicationId}.");
            }

            return Ok(result);
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