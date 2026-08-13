using Mony_Loop.Application.Common;
using Mony_Loop.Application.DTOs;

namespace Mony_Loop.Application.ServicesAbstractions
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