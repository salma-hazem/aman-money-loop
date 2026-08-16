using Microsoft.EntityFrameworkCore;
using Mony_Loop.Domain.Constants;
using Mony_Loop.Domain.Entities.CircleRequestManagement;
using Mony_Loop.Domain.Interfaces.CircleRequestManagement;
using Mony_Loop.Infrastructure.Data;

namespace Mony_Loop.Infrastructure.Repositories.CircleRequestManagement;

public sealed class MarketplaceListingRepository
    : IMarketplaceListingRepository
{
    private readonly MonyLoopDbContext _context;

    public MarketplaceListingRepository(MonyLoopDbContext context)
    {
        _context = context;
    }

    public Task<MarketplaceListing?> GetByIdAsync(
        Guid listingId,
        CancellationToken cancellationToken = default)
    {
        return _context.MarketplaceListings.FirstOrDefaultAsync(
            listing => listing.ListingId == listingId,
            cancellationToken);
    }

    public Task<MarketplaceListing?> GetByCircleIdAsync(
        Guid circleId,
        CancellationToken cancellationToken = default)
    {
        return _context.MarketplaceListings.FirstOrDefaultAsync(
            listing => listing.CircleId == circleId,
            cancellationToken);
    }

    public async Task<IReadOnlyList<MarketplaceListing>> GetActiveAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.MarketplaceListings
            .AsNoTracking()
            .Include(listing => listing.Circle)
            .Where(listing =>
                listing.ListingStatus == MarketplaceListingStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public Task AddAsync(
        MarketplaceListing listing,
        CancellationToken cancellationToken = default)
    {
        return _context.MarketplaceListings
            .AddAsync(listing, cancellationToken)
            .AsTask();
    }
}
