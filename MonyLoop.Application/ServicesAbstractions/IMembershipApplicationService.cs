using MonyLoop.Application.Common;
using MonyLoop.Application.DTOs;

namespace MonyLoop.Application.ServicesAbstractions
{
    public interface IMembershipApplicationService
    {
        Task<Result<MembershipApplicationDetailDto>> CreateApplicationAsync(
            CreateMembershipApplicationDto dto);

        Task<Result<MembershipApplicationDetailDto>> GetByIdAsync(
            Guid membershipApplicationId);

        Task<Result<IReadOnlyList<MembershipApplicationSummaryDto>>> GetByListingIdAsync(
            Guid listingId);

        Task<Result<MembershipApplicationDetailDto>> ShortlistAsync(
            Guid membershipApplicationId);

        Task<Result<MembershipApplicationDetailDto>> RejectAsync(
            Guid membershipApplicationId);
    }
}