using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonyLoop.Application.DTOs.OnboardingMemberLedger;
using MonyLoop.Application.ServicesAbstractions.OnboardingMemberLedger;
using MonyLoop.Domain.Entities.UserAuth;

namespace MonyLoop.API.Controllers.OnboardingMemberLedger
{
    [Authorize]
    public class MemberLedgersController : ApiBaseController
    {
        private readonly IMemberLedgerService _memberLedgerService;

        public MemberLedgersController(IMemberLedgerService memberLedgerService)
        {
            _memberLedgerService = memberLedgerService;
        }


        [Authorize(Roles = ApplicationRole.Admin)]
        [HttpPost("activate")]
        public async Task<ActionResult<MemberLedgerResponseDto>> Activate([FromBody] MemberLedgerRequestDto request, CancellationToken ct)
        {
            var uidClaim = User.FindFirst("uid")?.Value;
            if (uidClaim == null || !Guid.TryParse(uidClaim, out var adminId))
                return Unauthorized();

            request.ActivatedByAdminId = adminId;

            var result = await _memberLedgerService.ActivateAsync(request, adminId, ct);
            return HandleResult(result);
        }


        [Authorize(Roles = $"{ApplicationRole.Member},{ApplicationRole.Admin},{ApplicationRole.Organizer}")]
        [HttpGet("by-user/{userId:guid}")]
        public async Task<ActionResult<MemberLedgerResponseDto>> GetByUserId(Guid userId, CancellationToken ct)
        {
            var result = await _memberLedgerService.GetByUserIdAsync(userId, ct);
            return HandleResult(result);
        }
    }
}
