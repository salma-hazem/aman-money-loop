using AutoMapper;
using MonyLoop.Application.Common;
using MonyLoop.Application.DTOs.OnboardingMemberLedger;
using MonyLoop.Application.ServicesAbstractions.OnboardingMemberLedger;
using MonyLoop.Domain.Entities.Onboarding___Member_Ledger;
using MonyLoop.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MonyLoop.Application.Services.OnboardingMemberLedger
{
    public class MemberLedgerService : IMemberLedgerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IOnboardingCaseService _onboardingCaseService;

        public MemberLedgerService(IUnitOfWork unitOfWork, IMapper mapper, IOnboardingCaseService onboardingCaseService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _onboardingCaseService = onboardingCaseService;
        }

        public async Task<Result<MemberLedgerResponseDto>> ActivateAsync(MemberLedgerRequestDto request, Guid activatedByAdminId, CancellationToken ct = default)
        {
            if (request == null)
                return Result<MemberLedgerResponseDto>.Fail(Error.Validation("MemberLedger.NullRequest", "The member ledger request data cannot be null."));


            if (request.UserId == Guid.Empty)
                return Result<MemberLedgerResponseDto>.Fail(Error.Validation("MemberLedger.InvalidUserId", "A valid User ID must be provided."));

            var alreadyExists = await _unitOfWork.MemberLedgers.ExistsForUserAsync(request.UserId, ct);
            if (alreadyExists)
                return Result<MemberLedgerResponseDto>.Fail(Error.Failure("MemberLedger.AlreadyExists", "This user already has an active member ledger."));

            var memberLedger = _mapper.Map<MemberLedger>(request);

            await _unitOfWork.MemberLedgers.AddAsync(memberLedger, ct);


            var updateStatusResult =
                await _onboardingCaseService.MarkActivatedAsync(
                    request.OnboardingCaseId,
                    request.ActivatedByAdminId,
                    ct); 

            if (updateStatusResult.IsFailure)
                return Result<MemberLedgerResponseDto>.Fail(updateStatusResult.Errors.ToList());

            await _unitOfWork.SaveChangesAsync(ct);

            var responseDto = _mapper.Map<MemberLedgerResponseDto>(memberLedger);
            return (Result<MemberLedgerResponseDto>)responseDto;
        }

        public async Task<Result<MemberLedgerResponseDto>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            if (userId == Guid.Empty)
            {
                return Result<MemberLedgerResponseDto>.Fail(
                    Error.Validation("MemberLedger.InvalidUserId", "The provided user ID is invalid.")
                );
            }

            var memberLedger = await _unitOfWork.MemberLedgers.GetByUserIdAsync(userId, ct);
            if (memberLedger == null)
            {
                return Result<MemberLedgerResponseDto>.Fail(
                    Error.NotFound("MemberLedger.NotFound", $"The member ledger for user ID '{userId}' was not found.")
                );
            }

            var responseDto = _mapper.Map<MemberLedgerResponseDto>(memberLedger);
            return (Result<MemberLedgerResponseDto>)responseDto;
        }
    }
}