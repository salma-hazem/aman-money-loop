using MonyLoop.Domain.Entities.Marketplace___Applications;

namespace MonyLoop.Domain.Interfaces
{
    public interface IMembershipApplicationRepository
    {
        Task AddAsync(MembershipApplication application);
        Task<MembershipApplication?> GetByIdAsync(Guid membershipApplicationId);
        Task<List<MembershipApplication>> GetByListingIdAsync(Guid listingId);
        Task UpdateAsync(MembershipApplication application);
    }
}