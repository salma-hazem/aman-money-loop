using Microsoft.EntityFrameworkCore;
using MonyLoop.Domain.Entities.Marketplace___Applications;
using MonyLoop.Domain.Interfaces;
using MonyLoop.Infrastructure.Data;
namespace MonyLoop.Infrastructure.Repositories
{
    public class MembershipApplicationRepository : IMembershipApplicationRepository
    {
        private readonly MonyLoopDbContext _context;
        public MembershipApplicationRepository(MonyLoopDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(MembershipApplication application)
        {
            await _context.MembershipApplications.AddAsync(application);
            await _context.SaveChangesAsync();
        }
        public async Task<MembershipApplication?> GetByIdAsync(Guid membershipApplicationId)
        {
            return await _context.MembershipApplications
                .Include(a => a.MarketplaceListing)
                .FirstOrDefaultAsync(a => a.MembershipApplicationId == membershipApplicationId);
        }
        public async Task<(List<MembershipApplication> Items, int TotalCount)> GetByListingIdAsync(
            Guid listingId, int pageNumber, int pageSize)
        {
            var query = _context.MembershipApplications
                .Where(a => a.ListingId == listingId);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(a => a.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
        public async Task UpdateAsync(MembershipApplication application)
        {
            _context.MembershipApplications.Update(application);
            await _context.SaveChangesAsync();
        }
        public async Task<MembershipApplication?> GetByIdWithAgreementDetailsAsync(
    Guid membershipApplicationId)
        {
            return await _context.MembershipApplications
                .Include(a => a.MarketplaceListing)
                    .ThenInclude(l => l!.Circle)
                        .ThenInclude(c => c!.CircleRequest)
                .FirstOrDefaultAsync(
                    a => a.MembershipApplicationId == membershipApplicationId);
        }
    }
}