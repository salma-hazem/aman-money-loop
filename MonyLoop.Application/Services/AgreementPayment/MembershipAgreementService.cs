using AutoMapper;
using MonyLoop.Application.DTOs.AgreementPayment.MembershipAgreement;
using MonyLoop.Application.DTOs.OnboardingMemberLedger;
using MonyLoop.Application.ServicesAbstractions;
using MonyLoop.Application.ServicesAbstractions.UserAuth;
using MonyLoop.Application.ServicesAbstractions.AgreementPayment;
using MonyLoop.Application.ServicesAbstractions.OnboardingMemberLedger;
using MonyLoop.Domain.Constants;
using MonyLoop.Domain.Constants.Agreement___Payment;
using MonyLoop.Domain.Entities.Agreement___Payment;
using MonyLoop.Domain.Interfaces;
using MonyLoop.Domain.Interfaces.AgreementPayment;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using System.Text;


namespace MonyLoop.Application.Services.AgreementPayment
{
    public class MembershipAgreementService : IMembershipAgreementService
    {
        private readonly IMembershipApplicationRepository _membershipApplicationRepository;
        private readonly IMembershipAgreementRepository _membershipAgreementRepository;
        private readonly IOnboardingCaseService _onboardingCaseService;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public MembershipAgreementService(
        IMembershipAgreementRepository membershipAgreementRepository,
        IMembershipApplicationRepository membershipApplicationRepository,
        IOnboardingCaseService onboardingCaseService,
        IEmailSender emailSender,
        IConfiguration configuration,
        IUnitOfWork unitOfWork,
        IMapper mapper)
        {
            _membershipAgreementRepository = membershipAgreementRepository;
            _membershipApplicationRepository = membershipApplicationRepository;
            _onboardingCaseService = onboardingCaseService;
            _emailSender = emailSender;
            _configuration = configuration;
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
            //i assumed VerificationCompleted means Verification Completed + Selected 
            if (application.Stage != MembershipApplicationStage.VerificationCompleted)
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
            var responseToken = GenerateResponseToken();
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
                ResponseTokenHash =HashResponseToken(responseToken),
                RespondedAt = null
            };
            await _membershipAgreementRepository .AddAsync(agreement);
            application.Stage = MembershipApplicationStage.AgreementExtended;
            await _unitOfWork.SaveChangesAsync();
            var frontendBaseUrl =
    _configuration["EmailSettings:FrontendBaseUrl"];

            if (string.IsNullOrWhiteSpace(frontendBaseUrl))
            {
                throw new InvalidOperationException(
                    "Frontend base URL is not configured.");
            }

            var responseUrl =
                $"{frontendBaseUrl.TrimEnd('/')}/agreement-response" +
                $"?agreementId={agreement.MembershipAgreementId}" +
                $"&token={Uri.EscapeDataString(responseToken)}";

            var emailBody = $"""
            <p>Dear {application.Name},</p>

            <p>
                Your membership agreement for
                <strong>{agreement.CircleTitle}</strong>
                is ready for review.
            </p>

            <p>
                Please use the link below to review and respond to your agreement:
            </p>

            <p>
                <a href="{responseUrl}">
                    Review Membership Agreement
                </a>
            </p>

            <p>
                This agreement expires on
                <strong>{agreement.ExpiryDate:dd MMM yyyy}</strong>.
            </p>

            <p>Regards,<br/>MonyLoop Team</p>
            """;

                    await _emailSender.SendEmailAsync(
                        application.Email,
                        "Membership Agreement Ready for Review",
                        emailBody);
            return _mapper.Map<MembershipAgreementResponse>(agreement);
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

        public async Task<MembershipAgreementResponse?> AcceptAgreementAsync( Guid id, string token)
        {
            var agreement =
                await _membershipAgreementRepository.GetByIdAsync(id);

            if (agreement is null)
                return null;
            if (!IsResponseTokenValid( token, agreement.ResponseTokenHash))
            {
                throw new UnauthorizedAccessException(
                    "The agreement response link is invalid.");
            }

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

            // Get the related application and circle information
            var application =
                await _membershipApplicationRepository
                    .GetByIdWithAgreementDetailsAsync(
                        agreement.MembershipApplicationId);

            if (application is null)
            {
                throw new InvalidOperationException(
                    "The membership application related to this agreement was not found.");
            }

            var organizerId =
                application.MarketplaceListing?
                    .Circle?
                    .CircleRequest?
                    .CreatedByOrganizerId;

            if (organizerId is null || organizerId == Guid.Empty)
            {
                throw new InvalidOperationException(
                    "The organizer for this agreement could not be determined.");
            }

            // Accept agreement
            agreement.Status = AgreementStatus.Accepted;
            agreement.RespondedAt = DateTime.UtcNow;

            // Agreement Extended → Confirmed
            application.Stage = MembershipApplicationStage.Confirmed;
            _membershipAgreementRepository.Update(agreement);
            await _unitOfWork.SaveChangesAsync();

            // Trigger Module 6 onboarding
            var onboardingRequest = new OnboardingCaseRequestDto
            {
                MembershipAgreementId = agreement.MembershipAgreementId,
                OrganizerId = organizerId.Value
            };

            var onboardingResult =
                await _onboardingCaseService.CreateAsync(onboardingRequest);

            if (onboardingResult.IsFailure)
            {
                throw new InvalidOperationException(
                    "The agreement was accepted, but the onboarding case could not be created.");
            }

            return _mapper.Map<MembershipAgreementResponse>(agreement);
        }

        public async Task<MembershipAgreementResponse?> DeclineAgreementAsync(Guid id, string token)
        {
            var agreement =
                await _membershipAgreementRepository.GetByIdAsync(id);

            if (agreement is null)
                return null;

            if (!IsResponseTokenValid( token,agreement.ResponseTokenHash))
            {
                throw new UnauthorizedAccessException(
                    "The agreement response link is invalid.");
            }

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

            var application =
                await _membershipApplicationRepository
                    .GetByIdWithAgreementDetailsAsync(
                        agreement.MembershipApplicationId);

            if (application is null)
            {
                throw new InvalidOperationException(
                    "The membership application related to this agreement was not found.");
            }

            var circle =
                application.MarketplaceListing?.Circle;

            if (circle is null)
            {
                throw new InvalidOperationException(
                    "The circle related to this agreement could not be found.");
            }

            // Decline the agreement
            agreement.Status = AgreementStatus.Declined;
            agreement.RespondedAt = DateTime.UtcNow;

            _membershipAgreementRepository.Update(agreement);

            // Return the circle to recruitment
            circle.Status = CircleStatus.InRecruitment;

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<MembershipAgreementResponse>(agreement);
        }

        public async Task<MembershipAgreementResponse?> GetAgreementForResponseAsync(
        Guid id,
        string token)
        {
            var agreement =
                await _membershipAgreementRepository.GetByIdAsync(id);

            if (agreement is null)
                return null;

            if (!IsResponseTokenValid(
                token,
                agreement.ResponseTokenHash))
            {
                throw new UnauthorizedAccessException(
                    "The agreement response link is invalid.");
            }

            var isExpired =
                await MarkAsExpiredIfNeededAsync(agreement);

            if (isExpired)
            {
                throw new InvalidOperationException(
                    "This agreement has expired. Please contact Operations for assistance.");
            }

            return _mapper.Map<MembershipAgreementResponse>(
                agreement);
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
        private static string GenerateResponseToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(32);

            return Convert.ToHexString(bytes);
        }

        private static string HashResponseToken(string token)
        {
            var bytes = Encoding.UTF8.GetBytes(token);

            var hash = SHA256.HashData(bytes);

            return Convert.ToHexString(hash);
        }

        private static bool IsResponseTokenValid(
            string token,
            string storedTokenHash)
        {
            if (string.IsNullOrWhiteSpace(token) ||
                string.IsNullOrWhiteSpace(storedTokenHash))
            {
                return false;
            }

            var providedHash =
                SHA256.HashData(
                    Encoding.UTF8.GetBytes(token));

            var storedHash =
                Convert.FromHexString(storedTokenHash);

            return CryptographicOperations.FixedTimeEquals(
                providedHash,
                storedHash);
        }
    }
}