using MonyLoop.Domain.Entities.Marketplace___Applications;
namespace MonyLoop.Domain.Interfaces
{
    public interface IMembershipApplicationRepository
    {
        Task AddAsync(MembershipApplication application);
        Task<MembershipApplication?> GetByIdAsync(Guid membershipApplicationId);
        Task<(List<MembershipApplication> Items, int TotalCount)> GetByListingIdAsync(
            Guid listingId, int pageNumber, int pageSize);
        Task UpdateAsync(MembershipApplication application);
        Task<MembershipApplication?> GetByIdWithAgreementDetailsAsync(
            Guid membershipApplicationId);
        Task<List<MembershipApplication>> GetByUserIdAsync(Guid userId);
    }
}