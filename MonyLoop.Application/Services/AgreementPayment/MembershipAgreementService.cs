using AutoMapper;
using MonyLoop.Application.DTOs.AgreementPayment.MembershipAgreement;
using MonyLoop.Application.ServicesAbstractions.AgreementPayment;
using MonyLoop.Domain.Constants;
using MonyLoop.Domain.Constants.Agreement___Payment;
using MonyLoop.Domain.Entities.Agreement___Payment;
using MonyLoop.Domain.Interfaces;
using MonyLoop.Domain.Interfaces.AgreementPayment;

namespace MonyLoop.Application.Services.AgreementPayment
{
    public class MembershipAgreementService : IMembershipAgreementService
    {
        private readonly IMembershipAgreementRepository _membershipAgreementRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMembershipApplicationRepository _membershipApplicationRepository;

        public MembershipAgreementService(
        IMembershipAgreementRepository membershipAgreementRepository,
        IMembershipApplicationRepository membershipApplicationRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
        {
            _membershipAgreementRepository = membershipAgreementRepository;
            _membershipApplicationRepository = membershipApplicationRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<MembershipAgreementResponse> CreateAgreementAsync(
    CreateMembershipAgreementRequest request)
        {
            var application =
                await _membershipApplicationRepository
                    .GetByIdWithAgreementDetailsAsync(
                        request.MembershipApplicationId);
            if (application is null)
            {
                throw new KeyNotFoundException(
                    "Membership application was not found.");
            }
            // For the current implementation,
            // VerificationCompleted means Verification Completed + Selected. I'll make sure 
            if (application.Stage !=
                MembershipApplicationStage.VerificationCompleted)
            {
                throw new InvalidOperationException(
                    "An agreement can only be generated for a member who has completed verification and has been selected.");
            }

            var agreementAlreadyExists =await _membershipAgreementRepository
                    .ExistsForMembershipApplicationAsync(
                        request.MembershipApplicationId);

            if (agreementAlreadyExists)
            {
                throw new InvalidOperationException(
                    "A membership agreement already exists for this application.");
            }

            var today =DateOnly.FromDateTime(DateTime.UtcNow);
            if (request.ExpiryDate <= today)
            {
                throw new ArgumentException(
                    "Agreement expiry date must be a future date.");
            }
            if (request.StartDate > request.ExpiryDate)
            {
                throw new ArgumentException(
                    "Agreement start date cannot be after the expiry date.");
            }
            var listing = application.MarketplaceListing;
            if (listing is null)
            {
                throw new InvalidOperationException(
                    "Marketplace listing was not found for this application.");
            }
            var circle = listing.Circle;
            if (circle is null)
            {
                throw new InvalidOperationException(
                    "Circle was not found for this application.");
            }
            var circleRequest = circle.CircleRequest;
            if (circleRequest is null)
            {
                throw new InvalidOperationException(
                    "Circle request was not found for this circle.");
            }
            var agreement = new MembershipAgreement
            {
                MembershipAgreementId = Guid.NewGuid(),
                MembershipApplicationId = application.MembershipApplicationId,
                MemberName = application.Name,
                CircleTitle = circleRequest.CircleTitle,
                ContributionSchedule = request.ContributionSchedule,
                PayoutSlot = request.PayoutSlot,
                StartDate = request.StartDate,
                ExpiryDate = request.ExpiryDate,
                Status = AgreementStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                RespondedAt = null
            };
            await _membershipAgreementRepository .AddAsync(agreement);
            application.Stage = MembershipApplicationStage.AgreementExtended;
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<MembershipAgreementResponse>(
                agreement);
        }

        public async Task<MembershipAgreementResponse?> GetAgreementByIdAsync(Guid id)
        {
            var agreement =
                await _membershipAgreementRepository.GetByIdAsync(id);

            if (agreement is null)
                return null;

            await MarkAsExpiredIfNeededAsync(agreement);

            return _mapper.Map<MembershipAgreementResponse>(agreement);
        }

        public async Task<MembershipAgreementResponse?> AcceptAgreementAsync(Guid id)
        {
            var agreement =
                await _membershipAgreementRepository.GetByIdAsync(id);

            if (agreement is null)
                return null;

            var isExpired =
                await MarkAsExpiredIfNeededAsync(agreement);

            if (isExpired)
            {
                throw new InvalidOperationException(
                    "The agreement has expired and can no longer be accepted.");
            }

            if (agreement.Status != AgreementStatus.Pending)
            {
                throw new InvalidOperationException(
                    "Only pending agreements can be accepted.");
            }

            agreement.Status = AgreementStatus.Accepted;
            agreement.RespondedAt = DateTime.UtcNow;

            _membershipAgreementRepository.Update(agreement);

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<MembershipAgreementResponse>(agreement);
        }

        public async Task<MembershipAgreementResponse?> DeclineAgreementAsync(Guid id)
        {
            var agreement =
                await _membershipAgreementRepository.GetByIdAsync(id);

            if (agreement is null)
                return null;

            var isExpired =
                await MarkAsExpiredIfNeededAsync(agreement);

            if (isExpired)
            {
                throw new InvalidOperationException(
                    "The agreement has expired and can no longer be declined.");
            }

            if (agreement.Status != AgreementStatus.Pending)
            {
                throw new InvalidOperationException(
                    "Only pending agreements can be declined.");
            }

            agreement.Status = AgreementStatus.Declined;
            agreement.RespondedAt = DateTime.UtcNow;

            _membershipAgreementRepository.Update(agreement);

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<MembershipAgreementResponse>(agreement);
        }

        private async Task<bool> MarkAsExpiredIfNeededAsync(
            MembershipAgreement agreement)
        {
            // Already expired
            if (agreement.Status == AgreementStatus.Expired)
                return true;

            // Accepted or declined agreements should never later become expired
            if (agreement.Status != AgreementStatus.Pending)
                return false;

            var today =
                DateOnly.FromDateTime(DateTime.UtcNow);

            // Still valid
            if (agreement.ExpiryDate >= today)
                return false;

            agreement.Status = AgreementStatus.Expired;

            _membershipAgreementRepository.Update(agreement);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}