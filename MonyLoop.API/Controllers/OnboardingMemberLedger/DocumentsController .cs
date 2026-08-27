using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MonyLoop.Application.Common;
using MonyLoop.Application.DTOs;
using MonyLoop.Application.DTOs.OnboardingMemberLedger;
using MonyLoop.Application.ServicesAbstractions;
using MonyLoop.Application.ServicesAbstractions.OnboardingMemberLedger;
using MonyLoop.Domain.Entities.UserAuth;
using MonyLoop.API.Authentication;


namespace MonyLoop.API.Controllers.OnboardingMemberLedger
{
    [Authorize]
    public class DocumentsController : ApiBaseController
    {
        private readonly IDocumentService _documentService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IOnboardingCaseService _onboardingCaseService;

        public DocumentsController(
    IDocumentService documentService,
    IFileStorageService fileStorageService,
    IOnboardingCaseService onboardingCaseService)
        {
            _documentService = documentService;
            _fileStorageService = fileStorageService;
            _onboardingCaseService = onboardingCaseService;
        }

        [Authorize(Roles = ApplicationRole.Member)]
        [HttpPost("upload")]
        public async Task<ActionResult<DocumentResponseDto>> Upload([FromForm] Guid onboardingCaseId,
            [FromForm] Guid documentRequirementId, IFormFile file, CancellationToken ct)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");
            if (!CurrentUserIdResolver.TryGet(User, out var userId))
                return Unauthorized();

            var caseResult = await _onboardingCaseService.GetByIdAsync(onboardingCaseId, ct);

            if (caseResult.IsFailure)
                return NotFound("Onboarding case not found.");

            if (caseResult.Value.UserId != userId)
                return Forbid();
            const long maxFileSize = 10 * 1024 * 1024;

            if (file.Length > maxFileSize)
                return BadRequest("This file exceeds the maximum allowed size of 10 MB.");

            var extension = Path.GetExtension(file.FileName);

            if (!string.Equals(extension, ".pdf", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Only PDF files are allowed.");

            if (!string.Equals(file.ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Only PDF files are allowed.");


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

        [Authorize(Roles = $"{ApplicationRole.Admin},{ApplicationRole.Organizer}")]
        [HttpGet("{documentId:guid}/file")]
        public async Task<IActionResult> ViewFile(Guid documentId, CancellationToken ct)
        {
            var result = await _documentService.GetByIdAsync(documentId, ct);

            if (result.IsFailure)
                return HandleResult(result).Result!;
            if (User.IsInRole(ApplicationRole.Organizer) &&
    !User.IsInRole(ApplicationRole.Admin))
            {
                if (!CurrentUserIdResolver.TryGet(User, out var organizerId))
                    return Unauthorized();

                var caseResult =
                    await _onboardingCaseService.GetByIdAsync(
                        result.Value.OnboardingCaseId, ct);

                if (caseResult.IsFailure ||
                    caseResult.Value.OrganizerId != organizerId)
                    return Forbid();
            }

            var stream = _fileStorageService.OpenRead(result.Value.FilePath);

            if (stream == null)
                return NotFound("The document file could not be found.");

            return File(stream, "application/pdf");
        }

        [Authorize(Roles = $"{ApplicationRole.Member},{ApplicationRole.Admin},{ApplicationRole.Organizer}")]
        [HttpGet("by-case/{onboardingCaseId:guid}")]
        public async Task<ActionResult<IEnumerable<DocumentResponseDto>>> GetByOnboardingCaseId(Guid onboardingCaseId, CancellationToken ct)
        {
            if (User.IsInRole(ApplicationRole.Member) &&
    !User.IsInRole(ApplicationRole.Admin) &&
    !User.IsInRole(ApplicationRole.Organizer))
            {
                if (!CurrentUserIdResolver.TryGet(User, out var userId))
                    return Unauthorized();

                var caseResult = await _onboardingCaseService.GetByIdAsync(onboardingCaseId, ct);

                if (caseResult.IsFailure || caseResult.Value.UserId != userId)
                    return Forbid();
            }
            if (User.IsInRole(ApplicationRole.Organizer) &&
    !User.IsInRole(ApplicationRole.Admin))
            {
                if (!CurrentUserIdResolver.TryGet(User, out var organizerId))
                    return Unauthorized();

                var caseResult =
                    await _onboardingCaseService.GetByIdAsync(
                        onboardingCaseId, ct);

                if (caseResult.IsFailure ||
                    caseResult.Value.OrganizerId != organizerId)
                    return Forbid();
            }

            var result = await _documentService.GetByOnboardingCaseIdAsync(onboardingCaseId, ct);
            return HandleResult(result);
        }

        [Authorize(Roles = $"{ApplicationRole.Admin},{ApplicationRole.Organizer}")]
        [HttpGet("pending-review")]
        public async Task<ActionResult<PagedResult<DocumentResponseDto>>> GetPendingReview(
    [FromQuery] PaginationRequestDto pagination,
    CancellationToken ct)
        {
            if (User.IsInRole(ApplicationRole.Organizer) &&
                !User.IsInRole(ApplicationRole.Admin))
            {
                if (!CurrentUserIdResolver.TryGet(User, out var organizerId))
                    return Unauthorized();

                var organizerResult =
                    await _documentService.GetPendingReviewByOrganizerAsync(
                        organizerId,
                        pagination.PageNumber,
                        pagination.PageSize,
                        ct);

                return HandleResult(organizerResult);
            }

            var result = await _documentService.GetPendingReviewAsync(
                pagination.PageNumber,
                pagination.PageSize,
                ct);

            return HandleResult(result);
        }

        [Authorize(Roles = $"{ApplicationRole.Admin},{ApplicationRole.Organizer}")]
        [HttpPatch("review")]
        public async Task<ActionResult<DocumentResponseDto>> Review(
    [FromBody] DocumentReviewRequestDto request,
    CancellationToken ct)
        {
            if (!CurrentUserIdResolver.TryGet(User, out var reviewerId))
                return Unauthorized();

            if (User.IsInRole(ApplicationRole.Organizer) &&
                !User.IsInRole(ApplicationRole.Admin))
            {
                var documentResult =
                    await _documentService.GetByIdAsync(request.DocumentId, ct);

                if (documentResult.IsFailure)
                    return HandleResult(documentResult);

                var caseResult =
                    await _onboardingCaseService.GetByIdAsync(
                        documentResult.Value.OnboardingCaseId, ct);

                if (caseResult.IsFailure)
                    return NotFound("Onboarding case not found.");

                if (caseResult.Value.OrganizerId != reviewerId)
                    return Forbid();
            }

            var result =
                await _documentService.ReviewAsync(
                    request,
                    reviewerId,
                    ct);

            return HandleResult(result);
        }
    }
}