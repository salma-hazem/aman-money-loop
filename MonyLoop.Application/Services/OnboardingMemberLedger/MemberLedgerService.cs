using AutoMapper;
using MonyLoop.Application.Common;
using MonyLoop.Application.DTOs.OnboardingMemberLedger;
using MonyLoop.Application.ServicesAbstractions.OnboardingMemberLedger;
using MonyLoop.Domain.Entities.Onboarding___Member_Ledger;
using MonyLoop.Domain.Interfaces;
using MonyLoop.Application.ServicesAbstractions.CircleRequestManagement;
using MonyLoop.Domain.Interfaces.AgreementPayment;
using MonyLoop.Domain.Constants.Agreement___Payment;
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
        private readonly ISlotAssignmentService _slotAssignmentService;
        private readonly IMembershipAgreementRepository _membershipAgreementRepository;
        private readonly IMembershipApplicationRepository _membershipApplicationRepository;

        public MemberLedgerService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IOnboardingCaseService onboardingCaseService,
            ISlotAssignmentService slotAssignmentService,
            IMembershipAgreementRepository membershipAgreementRepository,
            IMembershipApplicationRepository membershipApplicationRepository)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _onboardingCaseService = onboardingCaseService;
            _slotAssignmentService = slotAssignmentService;
            _membershipAgreementRepository = membershipAgreementRepository;
            _membershipApplicationRepository = membershipApplicationRepository;
        }

        public async Task<Result<MemberLedgerResponseDto>> ActivateAsync(
    MemberLedgerRequestDto request,
    Guid activatedByAdminId,
    CancellationToken ct = default)
        {
            if (request == null)
            {
                return Result<MemberLedgerResponseDto>.Fail(
                    Error.Validation(
                        "MemberLedger.NullRequest",
                        "The member ledger request data cannot be null."));
            }

            if (request.UserId == Guid.Empty)
            {
                return Result<MemberLedgerResponseDto>.Fail(
                    Error.Validation(
                        "MemberLedger.InvalidUserId",
                        "A valid User ID must be provided."));
            }

            if (activatedByAdminId == Guid.Empty)
            {
                return Result<MemberLedgerResponseDto>.Fail(
                    Error.Validation(
                        "MemberLedger.InvalidAdminId",
                        "The authenticated Admin ID is invalid."));
            }

            var onboardingCase =
                await _unitOfWork.OnboardingCases.GetByIdAsync(
                    request.OnboardingCaseId,
                    ct);

            if (onboardingCase is null)
            {
                return Result<MemberLedgerResponseDto>.Fail(
                    Error.NotFound(
                        "OnboardingCase.NotFound",
                        "The onboarding case was not found."));
            }

            var agreement =
                await _membershipAgreementRepository.GetByIdAsync(
                    onboardingCase.MembershipAgreementId,
                    ct);

            if (agreement is null)
            {
                return Result<MemberLedgerResponseDto>.Fail(
                    Error.NotFound(
                        "MembershipAgreement.NotFound",
                        "The membership agreement was not found."));
            }

            if (agreement.Status != AgreementStatus.Accepted)
            {
                return Result<MemberLedgerResponseDto>.Fail(
                    Error.Validation(
                        "MembershipAgreement.NotAccepted",
                        "The membership agreement must be accepted before activating the member ledger."));
            }

            var application =
                await _membershipApplicationRepository
                    .GetByIdWithAgreementDetailsAsync(
                        agreement.MembershipApplicationId);

            if (application is null)
            {
                return Result<MemberLedgerResponseDto>.Fail(
                    Error.NotFound(
                        "MembershipApplication.NotFound",
                        "The membership application was not found."));
            }

            if (application.UserId != request.UserId)
            {
                return Result<MemberLedgerResponseDto>.Fail(
                    Error.Validation(
                        "MemberLedger.UserMismatch",
                        "The provided user does not belong to this membership application."));
            }

            var circle =
                application.MarketplaceListing?.Circle;

            if (circle is null)
            {
                return Result<MemberLedgerResponseDto>.Fail(
                    Error.NotFound(
                        "Circle.NotFound",
                        "The circle related to this membership agreement was not found."));
            }

            var alreadyExists =
                await _unitOfWork.MemberLedgers.ExistsForUserAsync(
                    request.UserId,
                    ct);

            if (alreadyExists)
            {
                return Result<MemberLedgerResponseDto>.Fail(
                    Error.Failure(
                        "MemberLedger.AlreadyExists",
                        "This user already has an active member ledger."));
            }

            var memberLedger = new MemberLedger
            {
                MemberLedgerId = Guid.NewGuid(),
                UserId = request.UserId,
                OnboardingCaseId = request.OnboardingCaseId,

                // Use the authenticated Admin, not an ID supplied by the client.
                ActivatedByAdminId = activatedByAdminId,

                ActivatedAt = DateTime.UtcNow
            };

            await _unitOfWork.MemberLedgers.AddAsync(
                memberLedger,
                ct);

            // Assign exactly the payout slot selected
            // in the membership agreement.
            var slotAssignmentResult =
                await _slotAssignmentService.AssignMemberLedgerAsync(
                    activatedByAdminId,
                    circle.CircleId,
                    agreement.PayoutSlot,
                    memberLedger.MemberLedgerId,
                    ct);

            if (slotAssignmentResult.IsFailure)
            {
                return Result<MemberLedgerResponseDto>.Fail(
                    slotAssignmentResult.Errors.ToList());
            }

            var updateStatusResult =
                await _onboardingCaseService.MarkActivatedAsync(
                    request.OnboardingCaseId,
                    activatedByAdminId,
                    ct);

            if (updateStatusResult.IsFailure)
            {
                return Result<MemberLedgerResponseDto>.Fail(
                    updateStatusResult.Errors.ToList());
            }

            await _unitOfWork.SaveChangesAsync(ct);

            var responseDto =
                _mapper.Map<MemberLedgerResponseDto>(
                    memberLedger);

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