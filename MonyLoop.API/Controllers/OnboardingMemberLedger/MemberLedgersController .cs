using Microsoft.AspNetCore.Mvc;
using MonyLoop.Application.DTOs.OnboardingMemberLedger;
using MonyLoop.Application.ServicesAbstractions.OnboardingMemberLedger;

namespace MonyLoop.API.Controllers.OnboardingMemberLedger
{
    public class MemberLedgersController : ApiBaseController
    {
        private readonly IMemberLedgerService _memberLedgerService;

        public MemberLedgersController(IMemberLedgerService memberLedgerService)
        {
            _memberLedgerService = memberLedgerService;
        }

        [HttpPost("activate")]
        public async Task<ActionResult<MemberLedgerResponseDto>> Activate([FromBody] MemberLedgerRequestDto request, CancellationToken ct)
        {
            var adminId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString()); // مؤقت لحد ما نخلص Auth/JWT
            var result = await _memberLedgerService.ActivateAsync(request, adminId, ct);
            return HandleResult(result);
        }

        [HttpGet("by-user/{userId:guid}")]
        public async Task<ActionResult<MemberLedgerResponseDto>> GetByUserId(Guid userId, CancellationToken ct)
        {
            var result = await _memberLedgerService.GetByUserIdAsync(userId, ct);
            return HandleResult(result);
        }
    }
}
