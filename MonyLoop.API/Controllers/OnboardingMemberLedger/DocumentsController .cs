using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MonyLoop.Application.Common;
using MonyLoop.Application.DTOs;
using MonyLoop.Application.DTOs.OnboardingMemberLedger;
using MonyLoop.Application.ServicesAbstractions;
using MonyLoop.Application.ServicesAbstractions.OnboardingMemberLedger;
using MonyLoop.Domain.Entities.UserAuth;

namespace MonyLoop.API.Controllers.OnboardingMemberLedger
{
    [Authorize]
    public class DocumentsController : ApiBaseController
    {
        private readonly IDocumentService _documentService;
        private readonly IFileStorageService _fileStorageService;

        public DocumentsController(IDocumentService documentService, IFileStorageService fileStorageService)
        {
            _documentService = documentService;
            _fileStorageService = fileStorageService;
        }

        [Authorize(Roles = ApplicationRole.Member)]
        [HttpPost("upload")]
        public async Task<ActionResult<DocumentResponseDto>> Upload([FromForm] Guid onboardingCaseId,
            [FromForm] Guid documentRequirementId, IFormFile file, CancellationToken ct)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var relativePath = await _fileStorageService.SaveAsync(file, "documents", ct);

            var request = new DocumentRequestDto
            {
                OnboardingCaseId = onboardingCaseId,
                DocumentRequirementId = documentRequirementId,
                FileName = file.FileName,
                FilePath = relativePath,
                FileSize = file.Length
            };

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
        public async Task<ActionResult<PagedResult<DocumentResponseDto>>> GetPendingReview([FromQuery] PaginationRequestDto pagination, CancellationToken ct)
        {
            var result = await _documentService.GetPendingReviewAsync(pagination.PageNumber, pagination.PageSize, ct);
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