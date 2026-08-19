using MonyLoop.Application.Common;
using MonyLoop.Application.DTOs;
using MonyLoop.Application.ServicesAbstractions;
using MonyLoop.Domain.Constants;
using MonyLoop.Domain.Entities.Marketplace___Applications;
using MonyLoop.Domain.Interfaces;

namespace MonyLoop.Application.Services
{
    public class MembershipApplicationService : IMembershipApplicationService
    {
        private readonly IMembershipApplicationRepository _repository;

        public MembershipApplicationService(IMembershipApplicationRepository repository)
        {
            _repository = repository;
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

        public async Task<Result<IReadOnlyList<MembershipApplicationSummaryDto>>> GetByListingIdAsync(
            Guid listingId)
        {
            var applications = await _repository.GetByListingIdAsync(listingId);

            var summaries = applications
                .Select(a => new MembershipApplicationSummaryDto
                {
                    MembershipApplicationId = a.MembershipApplicationId,
                    Name = a.Name,
                    Stage = a.Stage,
                    CreatedAt = a.CreatedAt
                })
                .ToList();

            return Result<IReadOnlyList<MembershipApplicationSummaryDto>>.Ok(summaries);
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

            return ToDetailDto(application);
        }

        private static MembershipApplicationDetailDto ToDetailDto(MembershipApplication a) =>
            new()
            {
                MembershipApplicationId = a.MembershipApplicationId,
                ListingId = a.ListingId,
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