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
    public class OnboardingCaseService : IOnboardingCaseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OnboardingCaseService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<OnboardingCaseResponseDto>> CreateAsync(OnboardingCaseRequestDto request, CancellationToken ct = default)
        {
            if (request == null)
                return Result<OnboardingCaseResponseDto>.Fail(Error.Validation("OnboardingCase.NullRequest", "The request data cannot be null."));

            if (request.OrganizerId == Guid.Empty)
                return Result<OnboardingCaseResponseDto>.Fail(Error.Validation("OnboardingCase.InvalidOrganizer", "A valid Organizer ID must be provided."));

            var onboardingCase = _mapper.Map<OnboardingCase>(request);

            await _unitOfWork.OnboardingCases.AddAsync(onboardingCase, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            var responseDto = _mapper.Map<OnboardingCaseResponseDto>(onboardingCase);
            return (Result<OnboardingCaseResponseDto>)responseDto;
        }

        public async Task<Result<OnboardingCaseResponseDto>> GetByIdAsync(Guid onboardingCaseId, CancellationToken ct = default)
        {
            if (onboardingCaseId == Guid.Empty)
                return Result<OnboardingCaseResponseDto>.Fail(Error.Validation("OnboardingCase.InvalidId", "The provided onboarding case ID is invalid."));


            var onboardingCase = await _unitOfWork.OnboardingCases.GetByIdAsync(onboardingCaseId, ct);

            if (onboardingCase == null)
                return Result<OnboardingCaseResponseDto>.Fail(Error.NotFound("OnboardingCase.NotFound", $"The onboarding case with ID '{onboardingCaseId}' was not found"));

            var responseDto = _mapper.Map<OnboardingCaseResponseDto>(onboardingCase);
            return (Result<OnboardingCaseResponseDto>)responseDto;
        }

        public async Task<Result<OnboardingCaseResponseDto>> GetByIdWithDocumentsAsync(Guid onboardingCaseId, CancellationToken ct = default)
        {
            if (onboardingCaseId == Guid.Empty)
                return Result<OnboardingCaseResponseDto>.Fail(Error.Validation("OnboardingCase.InvalidId", "The provided onboarding case ID is invalid."));


            var onboardingCase = await _unitOfWork.OnboardingCases.GetByIdWithDocumentsAsync(onboardingCaseId, ct);
            if (onboardingCase == null)
                return Result<OnboardingCaseResponseDto>.Fail(Error.NotFound("OnboardingCase.NotFound", $"The onboarding case with ID '{onboardingCaseId}' and its documents were not found."));

            var responseDto = _mapper.Map<OnboardingCaseResponseDto>(onboardingCase);
            return (Result<OnboardingCaseResponseDto>)responseDto;
        }

        public async Task<Result<IEnumerable<OnboardingCaseResponseDto>>> GetByOrganizerIdAsync(Guid organizerId, CancellationToken ct = default)
        {
            if (organizerId == Guid.Empty)
                return Result<IEnumerable<OnboardingCaseResponseDto>>.Fail(Error.Validation("Organizer.InvalidId", "The provided organizer ID is invalid."));


            var onboardingCases = await _unitOfWork.OnboardingCases.GetByOrganizerIdAsync(organizerId, ct);

            if (onboardingCases == null)
                return (Result<IEnumerable<OnboardingCaseResponseDto>>)Enumerable.Empty<OnboardingCaseResponseDto>();


            var responseDtos = _mapper.Map<IEnumerable<OnboardingCaseResponseDto>>(onboardingCases);

            return (Result<IEnumerable<OnboardingCaseResponseDto>>)responseDtos;
        }

        public async Task<Result<IEnumerable<OnboardingCaseResponseDto>>> GetByStatusAsync(OnboardingCaseStatus status, CancellationToken ct = default)
        {
            if (!Enum.IsDefined(typeof(OnboardingCaseStatus), status))
                return Result<IEnumerable<OnboardingCaseResponseDto>>.Fail(Error.Validation("OnboardingCase.InvalidStatus", "The provided onboarding status is invalid."));


            var onboardingCases = await _unitOfWork.OnboardingCases.GetByStatusAsync(status, ct);

            if (onboardingCases == null)
                return (Result<IEnumerable<OnboardingCaseResponseDto>>)Enumerable.Empty<OnboardingCaseResponseDto>();


            var responseDtos = _mapper.Map<IEnumerable<OnboardingCaseResponseDto>>(onboardingCases);

            return (Result<IEnumerable<OnboardingCaseResponseDto>>)responseDtos;
        }



        public async Task<Result> MarkActivatedAsync(Guid onboardingCaseId, Guid activatedByAdminId, CancellationToken ct = default)
        {
            if (onboardingCaseId == Guid.Empty)
            {
                return Result.Fail(Error.Validation("OnboardingCase.InvalidId", "The provided onboarding case ID is invalid."));
            }

            var onboardingCase = await _unitOfWork.OnboardingCases.GetByIdAsync(onboardingCaseId, ct);

            if (onboardingCase == null)
                return Result.Fail(Error.NotFound("OnboardingCase.NotFound", $"The onboarding case with ID '{onboardingCaseId}' was not found."));

            onboardingCase.FinalStatus = OnboardingCaseStatus.Approved;
            _unitOfWork.OnboardingCases.Update(onboardingCase);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Ok();
        }

        public async Task<Result> MarkDocumentsVerifiedAsync(Guid onboardingCaseId, CancellationToken ct = default)
        {
            if (onboardingCaseId == Guid.Empty)
                return Result.Fail(Error.Validation("OnboardingCase.InvalidId", "The provided onboarding case ID is invalid."));


            var onboardingCase = await _unitOfWork.OnboardingCases.GetByIdAsync(onboardingCaseId, ct);

            if (onboardingCase == null)
                return Result.Fail(Error.NotFound("OnboardingCase.NotFound", $"The onboarding case with ID '{onboardingCaseId}' was not found."));

            _unitOfWork.OnboardingCases.Update(onboardingCase);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Ok();
        }

        public async Task<Result> RecalculateAndUpdateStatusAsync(Guid onboardingCaseId, CancellationToken ct = default)
        {
            if (onboardingCaseId == Guid.Empty)
                return Result.Fail(Error.Validation("OnboardingCase.InvalidId", "The provided onboarding case ID is invalid."));

            var onboardingCase = await _unitOfWork.OnboardingCases.GetByIdAsync(onboardingCaseId, ct);
            if (onboardingCase == null)
                return Result.Fail(Error.NotFound("OnboardingCase.NotFound", $"The onboarding case with ID '{onboardingCaseId}' was not found."));

            var allVerified = await _unitOfWork.Documents.AllRequiredDocumentsVerifiedAsync(onboardingCaseId, ct);

            if (allVerified && onboardingCase.FinalStatus != OnboardingCaseStatus.Approved)
            {
                onboardingCase.FinalStatus = OnboardingCaseStatus.Approved;
                _unitOfWork.OnboardingCases.Update(onboardingCase);
                await _unitOfWork.SaveChangesAsync(ct);
            }

            return Result.Ok();
        }
    }
}