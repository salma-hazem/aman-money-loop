using AutoMapper;
using MonyLoop.Application.Common;
using MonyLoop.Application.DTOs.OnboardingMemberLedger;
using MonyLoop.Application.ServicesAbstractions.OnboardingMemberLedger;
using MonyLoop.Domain.Constants.Onboarding___Member_Ledger;
using MonyLoop.Domain.Entities.Onboarding___Member_Ledger;
using MonyLoop.Domain.Interfaces;
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

        public DocumentService(IUnitOfWork unitOfWork, IMapper mapper, IOnboardingCaseService onboardingCaseService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _onboardingCaseService = onboardingCaseService;
        }

        public async Task<Result<IEnumerable<DocumentResponseDto>>> GetByOnboardingCaseIdAsync(Guid onboardingCaseId, CancellationToken ct = default)
        {
            if (onboardingCaseId == Guid.Empty)
                return Result<IEnumerable<DocumentResponseDto>>.Fail(Error.Validation("Document.InvalidOnboardingCaseId", "The provided onboarding case ID is invalid."));


            var documents = await _unitOfWork.Documents.GetByOnboardingCaseIdAsync(onboardingCaseId, ct);

            if (documents == null)
                return (Result<IEnumerable<DocumentResponseDto>>)Enumerable.Empty<DocumentResponseDto>();


            var responseDtos = _mapper.Map<IEnumerable<DocumentResponseDto>>(documents);
            return (Result<IEnumerable<DocumentResponseDto>>)responseDtos;
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

        public async Task<Result<DocumentResponseDto>> ReviewAsync(DocumentReviewRequestDto request, CancellationToken ct = default)
        {
            if (request == null)
                return Result<DocumentResponseDto>.Fail(Error.Validation("Document.NullRequest", "The review request data cannot be null."));


            if (request.DocumentId == Guid.Empty)
                return Result<DocumentResponseDto>.Fail(Error.Validation("Document.InvalidId", "A valid Document ID must be provided for review."));

            // التحقق من صحة الـ Status قبل ما نعمل أي حاجة
            if (!Enum.TryParse<DocumentStatus>(request.NewStatus, out _))
                return Result<DocumentResponseDto>.Fail(Error.Validation("Document.InvalidStatus", "The provided status value is invalid."));



            var document = await _unitOfWork.Documents.GetByIdAsync(request.DocumentId, ct);
            if (document == null)
                return Result<DocumentResponseDto>.Fail(Error.NotFound("Document.NotFound", $"The document with ID '{request.DocumentId}' was not found."));


            _mapper.Map(request, document);

            _unitOfWork.Documents.Update(document);
            await _unitOfWork.SaveChangesAsync(ct);

            var recalcResult = await _onboardingCaseService.RecalculateAndUpdateStatusAsync(document.OnboardingCaseId, ct);
            if (recalcResult.IsFailure)
                return Result<DocumentResponseDto>.Fail(recalcResult.Errors.ToList());

            var responseDto = _mapper.Map<DocumentResponseDto>(document);
            return (Result<DocumentResponseDto>)responseDto;
        }

        public async Task<Result<DocumentResponseDto>> UploadAsync(DocumentRequestDto request, CancellationToken ct = default)
        {
            if (request == null)
                return Result<DocumentResponseDto>.Fail(Error.Validation("Document.NullRequest", "The document request data cannot be null."));


            if (request.OnboardingCaseId == Guid.Empty)
                return Result<DocumentResponseDto>.Fail(Error.Validation("Document.InvalidOnboardingCase", "A valid Onboarding Case ID must be provided."));


            var onboardingCase = await _unitOfWork.OnboardingCases.GetByIdAsync(request.OnboardingCaseId, ct);
            if (onboardingCase == null)
                return Result<DocumentResponseDto>.Fail(Error.NotFound("OnboardingCase.NotFound", $"The onboarding case with ID '{request.OnboardingCaseId}' was not found."));


            var document = _mapper.Map<Document>(request);

            await _unitOfWork.Documents.AddAsync(document, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            var responseDto = _mapper.Map<DocumentResponseDto>(document);
            return (Result<DocumentResponseDto>)responseDto;
        }
    }
}