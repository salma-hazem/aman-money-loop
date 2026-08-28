using MonyLoop.Application.Common;
using MonyLoop.Application.DTOs;
using MonyLoop.Application.ServicesAbstractions;
using MonyLoop.Application.ServicesAbstractions.UserAuth;
using MonyLoop.Domain.Constants;
using MonyLoop.Domain.Entities.Marketplace___Applications;
using MonyLoop.Domain.Interfaces;
using MonyLoop.Domain.Interfaces.CircleRequestManagement;

namespace MonyLoop.Application.Services
{
    public class MembershipApplicationService : IMembershipApplicationService
    {
        private readonly IMembershipApplicationRepository _repository;
        private readonly IEmailSender _emailSender;
        private readonly IMarketplaceListingRepository _listingRepository;

        public MembershipApplicationService(
        IMembershipApplicationRepository repository,
        IEmailSender emailSender,
        IMarketplaceListingRepository listingRepository)
        {
            _repository = repository;
            _emailSender = emailSender;
            _listingRepository = listingRepository;
        }

        public async Task<Result<MembershipApplicationDetailDto>> CreateApplicationAsync(
    CreateMembershipApplicationDto dto)
        {
            var listing = await _listingRepository.GetByIdAsync(dto.ListingId);

            if (listing is null)
                return Error.NotFound("MarketplaceListing.NotFound", "This circle listing could not be found.");

            if (listing.ListingStatus != MarketplaceListingStatus.Active)
                return Error.Validation("MarketplaceListing.NotAcceptingApplications",
                    "This circle is not currently accepting applications.");

            var application = new MembershipApplication
            {
                MembershipApplicationId = Guid.NewGuid(),
                ListingId = dto.ListingId,
                UserId = dto.UserId,
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                NationalId = dto.NationalId,
                Stage = MembershipApplicationStage.Submitted,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(application);

            return ToDetailDto(application);
        }

        public async Task<Result<MembershipApplicationDetailDto>> GetByIdAsync(
            Guid membershipApplicationId)
        {
            var application = await _repository.GetByIdAsync(membershipApplicationId);

            if (application is null)
                return Error.NotFound("MembershipApplication.NotFound", "Application not found.");

            return ToDetailDto(application);
        }

        public async Task<Result<PagedResult<MembershipApplicationSummaryDto>>> GetByListingIdAsync(
            Guid listingId, PaginationRequestDto pagination)
        {
            var (applications, totalCount) = await _repository.GetByListingIdAsync(
                listingId, pagination.PageNumber, pagination.PageSize);

            var summaries = applications
                .Select(a => new MembershipApplicationSummaryDto
                {
                    MembershipApplicationId = a.MembershipApplicationId,
                    Name = a.Name,
                    Stage = a.Stage,
                    CreatedAt = a.CreatedAt
                })
                .ToList();

            var pagedResult = new PagedResult<MembershipApplicationSummaryDto>
            {
                Items = summaries,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize,
                TotalCount = totalCount
            };

            return Result<PagedResult<MembershipApplicationSummaryDto>>.Ok(pagedResult);
        }

        public async Task<Result<MembershipApplicationDetailDto>> ShortlistAsync(
            Guid membershipApplicationId)
        {
            var application = await _repository.GetByIdAsync(membershipApplicationId);

            if (application is null)
                return Error.NotFound("MembershipApplication.NotFound", "Application not found.");

            if (application.Stage != MembershipApplicationStage.Submitted)
                return Error.Validation("MembershipApplication.InvalidTransition",
                    $"Cannot shortlist an application in stage '{application.Stage}'.");

            application.Stage = MembershipApplicationStage.Shortlisted;
            application.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(application);

            var isArabic = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar";
            await _emailSender.SendEmailAsync(
                application.Email,
                isArabic ? "تحديث حالة طلب العضوية" : "Membership Application Status Update",
                isArabic
                ? $"""
                <div dir="rtl" style="text-align:right">
                    <p>عزيزي/ عزيزتي {application.Name}،</p>
                    <p>تم تحديث حالة طلب عضويتك إلى <strong>القائمة المختصرة</strong>.</p>
                    <p>مع تحياتنا،<br/>فريق أمان ماني لوب</p>
                </div>
                """
                : $"""
                <p>Dear {application.Name},</p>
                <p>
                    Your membership application status has been updated to
                    <strong>{application.Stage}</strong>.
                </p>
                <p>Regards,<br/>MonyLoop Team</p>
                """);

            return ToDetailDto(application);
        }

        public async Task<Result<MembershipApplicationDetailDto>> RejectAsync(
            Guid membershipApplicationId)
        {
            var application = await _repository.GetByIdAsync(membershipApplicationId);

            if (application is null)
                return Error.NotFound("MembershipApplication.NotFound", "Application not found.");

            if (application.Stage == MembershipApplicationStage.Confirmed ||
                application.Stage == MembershipApplicationStage.Rejected)
                return Error.Validation("MembershipApplication.InvalidTransition",
                    $"Cannot reject an application in stage '{application.Stage}'.");

            application.Stage = MembershipApplicationStage.Rejected;
            application.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(application);

            var isArabic = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar";
            await _emailSender.SendEmailAsync(
                application.Email,
                isArabic ? "تحديث حالة طلب العضوية" : "Membership Application Status Update",
                isArabic
                ? $"""
                <div dir="rtl" style="text-align:right">
                    <p>عزيزي/ عزيزتي {application.Name}،</p>
                    <p>تم تحديث حالة طلب عضويتك إلى <strong>مرفوض</strong>.</p>
                    <p>مع تحياتنا،<br/>فريق أمان ماني لوب</p>
                </div>
                """
                : $"""
                <p>Dear {application.Name},</p>
                <p>
                    Your membership application status has been updated to
                    <strong>{application.Stage}</strong>.
                </p>
                <p>Regards,<br/>MonyLoop Team</p>
                """);

            return ToDetailDto(application);
        }

        public async Task<Result<IReadOnlyList<MembershipApplicationDetailDto>>> GetMyApplicationsAsync(
     Guid userId)
        {
            var applications = await _repository.GetByUserIdAsync(userId);

            var dtos = applications.Select(ToDetailDto).ToList();

            return Result<IReadOnlyList<MembershipApplicationDetailDto>>.Ok(dtos);
        }

        private static MembershipApplicationDetailDto ToDetailDto(MembershipApplication a) =>
    new()
    {
        MembershipApplicationId = a.MembershipApplicationId,
        ListingId = a.ListingId,
        CircleId = a.MarketplaceListing?.CircleId ?? Guid.Empty,
        Title = a.MarketplaceListing?.Circle?.CircleRequest?.CircleTitle,
        Name = a.Name,
        Email = a.Email,
        Phone = a.Phone,
        NationalId = a.NationalId,
        Stage = a.Stage,
        CreatedAt = a.CreatedAt,
        UpdatedAt = a.UpdatedAt
    };
    }
}
