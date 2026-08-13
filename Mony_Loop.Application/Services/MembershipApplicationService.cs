using Mony_Loop.Application.Common;
using Mony_Loop.Application.DTOs;
using Mony_Loop.Application.ServicesAbstractions;
using Mony_Loop.Domain.Constants;
using Mony_Loop.Domain.Entities.Marketplace___Applications;
using Mony_Loop.Domain.Interfaces;

namespace Mony_Loop.Application.Services
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

            return Result.Success(ToDetailDto(application));
        }

        public async Task<Result<MembershipApplicationDetailDto>> GetByIdAsync(
            Guid membershipApplicationId)
        {
            var application = await _repository.GetByIdAsync(membershipApplicationId);

            if (application is null)
                return Result.Failure<MembershipApplicationDetailDto>(
                    Error.NotFound("MembershipApplication.NotFound", "Application not found."));

            return Result.Success(ToDetailDto(application));
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

            return Result.Success<IReadOnlyList<MembershipApplicationSummaryDto>>(summaries);
        }

        public async Task<Result<MembershipApplicationDetailDto>> ShortlistAsync(
            Guid membershipApplicationId)
        {
            var application = await _repository.GetByIdAsync(membershipApplicationId);

            if (application is null)
                return Result.Failure<MembershipApplicationDetailDto>(
                    Error.NotFound("MembershipApplication.NotFound", "Application not found."));

            if (application.Stage != MembershipApplicationStage.Submitted)
                return Result.Failure<MembershipApplicationDetailDto>(
                    Error.Conflict("MembershipApplication.InvalidTransition",
                        $"Cannot shortlist an application in stage '{application.Stage}'."));

            application.Stage = MembershipApplicationStage.Shortlisted;
            application.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(application);

            return Result.Success(ToDetailDto(application));
        }

        public async Task<Result<MembershipApplicationDetailDto>> RejectAsync(
            Guid membershipApplicationId)
        {
            var application = await _repository.GetByIdAsync(membershipApplicationId);

            if (application is null)
                return Result.Failure<MembershipApplicationDetailDto>(
                    Error.NotFound("MembershipApplication.NotFound", "Application not found."));

            if (application.Stage == MembershipApplicationStage.Confirmed ||
                application.Stage == MembershipApplicationStage.Rejected)
                return Result.Failure<MembershipApplicationDetailDto>(
                    Error.Conflict("MembershipApplication.InvalidTransition",
                        $"Cannot reject an application in stage '{application.Stage}'."));

            application.Stage = MembershipApplicationStage.Rejected;
            application.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(application);

            return Result.Success(ToDetailDto(application));
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