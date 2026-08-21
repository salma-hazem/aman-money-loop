using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonyLoop.Application.DTOs.OnboardingMemberLedger;
using MonyLoop.Application.ServicesAbstractions.OnboardingMemberLedger;
using MonyLoop.Domain.Entities.UserAuth;

namespace MonyLoop.API.Controllers.OnboardingMemberLedger
{
    [Authorize]
    public class DocumentsController : ApiBaseController
    {
        private readonly IDocumentService _documentService;

        public DocumentsController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [Authorize(Roles = ApplicationRole.Member)]
        [HttpPost]
        public async Task<ActionResult<DocumentResponseDto>> Upload([FromBody] DocumentRequestDto request, CancellationToken ct)
        {
            var result = await _documentService.UploadAsync(request, ct);
            return HandleResult(result);
        }


        [Authorize(Roles = $"{ApplicationRole.Member},{ApplicationRole.Admin},{ApplicationRole.Organizer}")]
        [HttpGet("by-case/{onboardingCaseId:guid}")]
        public async Task<ActionResult<IEnumerable<DocumentResponseDto>>> GetByOnboardingCaseId(Guid onboardingCaseId, CancellationToken ct)
        {
            var result = await _documentService.GetByOnboardingCaseIdAsync(onboardingCaseId, ct);
            return HandleResult(result);
        }


        [Authorize(Roles = $"{ApplicationRole.Admin},{ApplicationRole.Organizer}")]
        [HttpGet("pending-review")]
        public async Task<ActionResult<IEnumerable<DocumentResponseDto>>> GetPendingReview(CancellationToken ct)
        {
            var result = await _documentService.GetPendingReviewAsync(ct);
            return HandleResult(result);
        }


        [Authorize(Roles = $"{ApplicationRole.Admin},{ApplicationRole.Organizer}")]
        [HttpPatch("review")]
        public async Task<ActionResult<DocumentResponseDto>> Review([FromBody] DocumentReviewRequestDto request, CancellationToken ct)
        {
            var result = await _documentService.ReviewAsync(request, ct);
            return HandleResult(result);
        }
    }
}
