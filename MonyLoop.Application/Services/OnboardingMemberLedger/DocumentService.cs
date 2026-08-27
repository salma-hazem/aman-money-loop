using AutoMapper;
using MonyLoop.Application.Common;
using MonyLoop.Application.DTOs.OnboardingMemberLedger;
using MonyLoop.Application.ServicesAbstractions.OnboardingMemberLedger;
using MonyLoop.Domain.Constants.Onboarding___Member_Ledger;
using MonyLoop.Domain.Entities.Onboarding___Member_Ledger;
using MonyLoop.Domain.Interfaces;
using MonyLoop.Application.ServicesAbstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MonyLoop.Application.ServicesAbstractions.UserAuth;
using MonyLoop.Domain.Entities.UserAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MonyLoop.Application.Services.OnboardingMemberLedger
{
    public class DocumentService : IDocumentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IOnboardingCaseService _onboardingCaseService;
        private readonly IFileStorageService _fileStorageService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<DocumentService> _logger;

        public DocumentService(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IOnboardingCaseService onboardingCaseService,
    IFileStorageService fileStorageService,
    UserManager<ApplicationUser> userManager,
    IEmailSender emailSender,
    ILogger<DocumentService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _onboardingCaseService = onboardingCaseService;
            _fileStorageService = fileStorageService;
            _userManager = userManager;
            _emailSender = emailSender;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<DocumentResponseDto>>> GetByOnboardingCaseIdAsync(Guid onboardingCaseId, CancellationToken ct = default)
        {
            if (onboardingCaseId == Guid.Empty)
                return Result<IEnumerable<DocumentResponseDto>>.Fail(Error.Validation("Document.InvalidOnboardingCaseId", "The provided onboarding case ID is invalid."));


            var documents = await _unitOfWork.Documents.GetByOnboardingCaseIdAsync(onboardingCaseId, ct);

            if (documents == null)
                return (Result<IEnumerable<DocumentResponseDto>>)Enumerable.Empty<DocumentResponseDto>();


            var responseDtos = _mapper.Map<List<DocumentResponseDto>>(documents);

            return Result<IEnumerable<DocumentResponseDto>>.Ok(responseDtos);
        }


        public async Task<Result<DocumentResponseDto>> GetByIdAsync(Guid documentId, CancellationToken ct = default)
        {
            if (documentId == Guid.Empty)
                return Result<DocumentResponseDto>.Fail(
                    Error.Validation("Document.InvalidId", "A valid Document ID must be provided."));

            var document = await _unitOfWork.Documents.GetByIdAsync(documentId, ct);

            if (document == null)
                return Result<DocumentResponseDto>.Fail(
                    Error.NotFound("Document.NotFound", $"Document '{documentId}' was not found."));

            return _mapper.Map<DocumentResponseDto>(document);
        }
        public async Task<Result<PagedResult<DocumentResponseDto>>> GetPendingReviewAsync(int pageNumber, int pageSize, CancellationToken ct = default)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var (items, totalCount) = await _unitOfWork.Documents.GetPendingReviewPagedAsync(pageNumber, pageSize, ct);
            var dtoItems = _mapper.Map<List<DocumentResponseDto>>(items);

            var pagedResult = new PagedResult<DocumentResponseDto>
            {
                Items = dtoItems,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return (Result<PagedResult<DocumentResponseDto>>)pagedResult;
        }

        public async Task<Result<DocumentResponseDto>> ReviewAsync(DocumentReviewRequestDto request, Guid reviewedByUserId, CancellationToken ct = default)
        {
            if (request == null)
                return Result<DocumentResponseDto>.Fail(Error.Validation("Document.NullRequest", "The review request data cannot be null."));
            
            if (reviewedByUserId == Guid.Empty)
                return Result<DocumentResponseDto>.Fail(
                    Error.Validation(
                        "Document.InvalidReviewer",
                        "A valid reviewer user ID is required."));

            if (request.DocumentId == Guid.Empty)
                return Result<DocumentResponseDto>.Fail(Error.Validation("Document.InvalidId", "A valid Document ID must be provided for review."));

            // التحقق من صحة الـ Status قبل ما نعمل أي حاجة
            if (!Enum.TryParse<DocumentStatus>(request.NewStatus, out _))
                return Result<DocumentResponseDto>.Fail(Error.Validation("Document.InvalidStatus", "The provided status value is invalid."));



            var document = await _unitOfWork.Documents.GetByIdAsync(request.DocumentId, ct);
            if (document == null)
                return Result<DocumentResponseDto>.Fail(Error.NotFound("Document.NotFound", $"The document with ID '{request.DocumentId}' was not found."));


            _mapper.Map(request, document);
            document.ReviewedByUserId = reviewedByUserId;

            _unitOfWork.Documents.Update(document);
            await _unitOfWork.SaveChangesAsync(ct);

            var recalcResult = await _onboardingCaseService.RecalculateAndUpdateStatusAsync(document.OnboardingCaseId, ct);
            if (recalcResult.IsFailure)
                return Result<DocumentResponseDto>.Fail(recalcResult.Errors.ToList());

            var responseDto = _mapper.Map<DocumentResponseDto>(document);
            return (Result<DocumentResponseDto>)responseDto;
        }

        public async Task<Result<DocumentResponseDto>> UploadAsync(
    DocumentRequestDto request,
    CancellationToken ct = default)
        {
            if (request == null)
                return Result<DocumentResponseDto>.Fail(
                    Error.Validation("Document.NullRequest", "The document request data cannot be null."));

            if (request.OnboardingCaseId == Guid.Empty)
                return Result<DocumentResponseDto>.Fail(
                    Error.Validation("Document.InvalidOnboardingCase", "A valid Onboarding Case ID must be provided."));

            if (request.DocumentRequirementId == Guid.Empty)
                return Result<DocumentResponseDto>.Fail(
                    Error.Validation("Document.InvalidRequirement", "A valid Document Requirement ID must be provided."));

            var onboardingCase =
                await _unitOfWork.OnboardingCases.GetByIdAsync(request.OnboardingCaseId, ct);

            if (onboardingCase == null)
                return Result<DocumentResponseDto>.Fail(
                    Error.NotFound("OnboardingCase.NotFound",
                        $"The onboarding case with ID '{request.OnboardingCaseId}' was not found."));

            var existingDocument =
                await _unitOfWork.Documents.GetByCaseAndRequirementAsync(
                    request.OnboardingCaseId,
                    request.DocumentRequirementId,
                    ct);

            if (existingDocument != null)
            {
                var oldFilePath = existingDocument.FilePath;

                existingDocument.FileName = request.FileName;
                existingDocument.FilePath = request.FilePath;
                existingDocument.FileSize = request.FileSize;
                existingDocument.Status = DocumentStatus.Pending;
                existingDocument.UploadedAt = DateTime.UtcNow;
                existingDocument.ReviewedByUserId = null;
                existingDocument.ReviewedAt = null;
                existingDocument.RejectionReason = null;

                _unitOfWork.Documents.Update(existingDocument);
                await _unitOfWork.SaveChangesAsync(ct);

                if (!string.IsNullOrWhiteSpace(oldFilePath) &&
    oldFilePath != request.FilePath)
                {
                    _fileStorageService.Delete(oldFilePath);
                }

                await NotifyOrganizerAsync(
                    onboardingCase.OrganizerId,
                    onboardingCase.OnboardingCaseId,
                    request.FileName,
                    ct);

                var replacedDto = _mapper.Map<DocumentResponseDto>(existingDocument);

                return (Result<DocumentResponseDto>)replacedDto;
            }

            var document = _mapper.Map<Document>(request);

            await _unitOfWork.Documents.AddAsync(document, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            await NotifyOrganizerAsync(
                onboardingCase.OrganizerId,
                onboardingCase.OnboardingCaseId,
                request.FileName,
                ct);

            var responseDto = _mapper.Map<DocumentResponseDto>(document);

            return (Result<DocumentResponseDto>)responseDto;
        }

        private async Task NotifyOrganizerAsync(
    Guid organizerId,
    Guid onboardingCaseId,
    string fileName,
    CancellationToken ct)
        {
            try
            {
                var organizer = await _userManager.FindByIdAsync(organizerId.ToString());

                if (organizer == null || string.IsNullOrWhiteSpace(organizer.Email))
                {
                    _logger.LogWarning(
                        "Organizer email could not be found for onboarding case {OnboardingCaseId}.",
                        onboardingCaseId);
                    return;
                }

                var subject = "New Onboarding Document Uploaded";

                var body = $"""
        <p>Dear {organizer.FirstName},</p>

        <p>A member has uploaded an onboarding document that is ready for review.</p>

        <p>
            <strong>File:</strong> {fileName}<br/>
            <strong>Onboarding Case ID:</strong> {onboardingCaseId}
        </p>

        <p>Please log in to MonyLoop to review the document.</p>

        <p>Regards,<br/>MonyLoop Team</p>
        """;

                await _emailSender.SendEmailAsync(
                    organizer.Email,
                    subject,
                    body,
                    ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to notify organizer {OrganizerId} for onboarding case {OnboardingCaseId}.",
                    organizerId,
                    onboardingCaseId);
            }
        }
        public async Task<Result<PagedResult<DocumentResponseDto>>> GetPendingReviewByOrganizerAsync(
    Guid organizerId,
    int pageNumber,
    int pageSize,
    CancellationToken ct = default)
        {
            if (organizerId == Guid.Empty)
                return Result<PagedResult<DocumentResponseDto>>.Fail(
                    Error.Validation("Document.InvalidOrganizer", "A valid Organizer ID is required."));

            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var (items, totalCount) =
                await _unitOfWork.Documents.GetPendingReviewByOrganizerPagedAsync(
                    organizerId, pageNumber, pageSize, ct);

            return new PagedResult<DocumentResponseDto>
            {
                Items = _mapper.Map<List<DocumentResponseDto>>(items),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
    }
}