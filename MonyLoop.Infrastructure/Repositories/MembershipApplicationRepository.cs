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
                .FirstOrDefaultAsync(a => a.MembershipApplicationId == membershipApplicationId);
        }

        public async Task<List<MembershipApplication>> GetByListingIdAsync(Guid listingId)
        {
            return await _context.MembershipApplications
                .Where(a => a.ListingId == listingId)
                .ToListAsync();
        }

        public async Task UpdateAsync(MembershipApplication application)
        {
            _context.MembershipApplications.Update(application);
            await _context.SaveChangesAsync();
        }
    }
}