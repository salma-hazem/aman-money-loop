using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MonyLoop.Application.DTOs.OnboardingMemberLedger;
using MonyLoop.Application.ServicesAbstractions.OnboardingMemberLedger;
using MonyLoop.Domain.Constants.Onboarding___Member_Ledger;
using MonyLoop.Domain.Entities.UserAuth;

namespace MonyLoop.API.Controllers.OnboardingMemberLedger
{
    [Authorize]
    public class OnboardingCasesController : ApiBaseController
    {
        private readonly IOnboardingCaseService _onboardingCaseService;

        public OnboardingCasesController(IOnboardingCaseService onboardingCaseService)
        {
            _onboardingCaseService = onboardingCaseService;
        }


        [Authorize(Roles = ApplicationRole.Member)]
        [HttpPost]
        public async Task<ActionResult<OnboardingCaseResponseDto>> Create([FromBody] OnboardingCaseRequestDto request, CancellationToken ct)
        {
            var result = await _onboardingCaseService.CreateAsync(request, ct);
            return HandleResult(result);
        }


        [Authorize(Roles = $"{ApplicationRole.Member},{ApplicationRole.Admin},{ApplicationRole.Organizer}")]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<OnboardingCaseResponseDto>> GetById(Guid id, CancellationToken ct)
        {
            var result = await _onboardingCaseService.GetByIdAsync(id, ct);
            return HandleResult(result);
        }


        [Authorize(Roles = $"{ApplicationRole.Admin},{ApplicationRole.Organizer}")]
        [HttpGet("{id:guid}/with-documents")]
        public async Task<ActionResult<OnboardingCaseResponseDto>> GetByIdWithDocuments(Guid id, CancellationToken ct)
        {
            var result = await _onboardingCaseService.GetByIdWithDocumentsAsync(id, ct);
            return HandleResult(result);
        }



        [Authorize(Roles = ApplicationRole.Organizer)]
        [HttpGet("by-organizer/{organizerId:guid}")]
        public async Task<ActionResult<IEnumerable<OnboardingCaseResponseDto>>> GetByOrganizer(Guid organizerId, CancellationToken ct)
        {
            var result = await _onboardingCaseService.GetByOrganizerIdAsync(organizerId, ct);
            return HandleResult(result);
        }


        [Authorize(Roles = $"{ApplicationRole.Admin},{ApplicationRole.Organizer}")]
        [HttpGet("by-status/{status}")]
        public async Task<ActionResult<IEnumerable<OnboardingCaseResponseDto>>> GetByStatus(OnboardingCaseStatus status, CancellationToken ct)
        {
            var result = await _onboardingCaseService.GetByStatusAsync(status, ct);
            return HandleResult(result);
        }


        [Authorize(Roles = $"{ApplicationRole.Admin},{ApplicationRole.Organizer}")]
        [HttpPatch("{id:guid}/mark-documents-verified")]
        public async Task<IActionResult> MarkDocumentsVerified(Guid id, CancellationToken ct)
        {
            var result = await _onboardingCaseService.MarkDocumentsVerifiedAsync(id, ct);
            return HandleResult(result);
        }
    }
}
