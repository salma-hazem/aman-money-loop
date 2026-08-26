using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MonyLoop.Application.DTOs.Verification;
using MonyLoop.Application.ServicesAbstractions.Verification;
using Microsoft.AspNetCore.Authorization;




namespace MonyLoop.Api.Controllers.Verification
{
    [ApiController]
    [Route("api/verification-rounds")]
    [Authorize]
    public class VerificationRoundController : ControllerBase
    {
        private readonly IVerificationRoundService _roundService;

        public VerificationRoundController(IVerificationRoundService roundService)
        {
            _roundService = roundService;
        }

        [HttpPost]
        public async Task<ActionResult<VerificationRoundResponseDto>> CreateRound([FromBody] CreateVerificationRoundDto dto, CancellationToken ct)
        {
            var result = await _roundService.CreateRoundAsync(dto, ct);
            return CreatedAtAction(nameof(GetRoundById), new { id = result.VerificationRoundId }, result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<VerificationRoundResponseDto>> GetRoundById([FromRoute] Guid id, CancellationToken ct)
        {
            var round = await _roundService.GetRoundByIdAsync(id, ct);
            if (round == null)
            {
                return NotFound();
            }

            return Ok(round);
        }

        [HttpGet("circle/{circleId:guid}")]
        public async Task<ActionResult<IReadOnlyList<VerificationRoundResponseDto>>> GetRoundsByCircle([FromRoute] Guid circleId, CancellationToken ct)
        {
            var rounds = await _roundService.GetRoundsByCircleIdAsync(circleId, ct);
            return Ok(rounds);
        }
    }
}
