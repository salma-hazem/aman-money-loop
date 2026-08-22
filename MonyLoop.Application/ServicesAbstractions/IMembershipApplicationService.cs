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
        Task<Result<PagedResult<MembershipApplicationSummaryDto>>> GetByListingIdAsync(
            Guid listingId, PaginationRequestDto pagination);
        Task<Result<MembershipApplicationDetailDto>> ShortlistAsync(
            Guid membershipApplicationId);
        Task<Result<MembershipApplicationDetailDto>> RejectAsync(
            Guid membershipApplicationId);
    }
}