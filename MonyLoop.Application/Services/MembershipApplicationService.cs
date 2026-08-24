using MonyLoop.Application.Common;
using MonyLoop.Application.DTOs;
using MonyLoop.Application.ServicesAbstractions;
using MonyLoop.Application.ServicesAbstractions.UserAuth;
using MonyLoop.Domain.Constants;
using MonyLoop.Domain.Entities.Marketplace___Applications;
using MonyLoop.Domain.Interfaces;

namespace MonyLoop.Application.Services
{
    public class MembershipApplicationService : IMembershipApplicationService
    {
        private readonly IMembershipApplicationRepository _repository;
        private readonly IEmailSender _emailSender;

        public MembershipApplicationService(
        IMembershipApplicationRepository repository,
        IEmailSender emailSender)
        {
            _repository = repository;
            _emailSender = emailSender;
        }

        public async Task<Result<MembershipApplicationDetailDto>> CreateApplicationAsync(
            CreateMembershipApplicationDto dto)
        {
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

            await _emailSender.SendEmailAsync(
                application.Email,
                "Membership Application Status Update",
                $"""
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

            await _emailSender.SendEmailAsync(
                application.Email,
                "Membership Application Status Update",
                $"""
                <p>Dear {application.Name},</p>
                <p>
                    Your membership application status has been updated to
                    <strong>{application.Stage}</strong>.
                </p>
                <p>Regards,<br/>MonyLoop Team</p>
                """);

            return ToDetailDto(application);
        }

        private static MembershipApplicationDetailDto ToDetailDto(MembershipApplication a) =>
    new()
    {
        MembershipApplicationId = a.MembershipApplicationId,
        ListingId = a.ListingId,
        CircleId = a.MarketplaceListing?.CircleId ?? Guid.Empty,   
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