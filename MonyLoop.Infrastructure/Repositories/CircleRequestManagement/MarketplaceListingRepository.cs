using Microsoft.EntityFrameworkCore;
using MonyLoop.Domain.Constants;
using MonyLoop.Domain.Entities.CircleRequestManagement;
using MonyLoop.Domain.Interfaces.CircleRequestManagement;
using MonyLoop.Infrastructure.Data;

namespace MonyLoop.Infrastructure.Repositories.CircleRequestManagement;

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

    public Task<MarketplaceListing?> GetDetailsByIdAsync(
        Guid listingId,
        CancellationToken cancellationToken = default)
    {
        return _context.MarketplaceListings
            .AsNoTracking()
            .Include(listing => listing.Circle)
                .ThenInclude(circle => circle!.CircleRequest)
            .FirstOrDefaultAsync(
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
         .ThenInclude(circle => circle!.CircleRequest)
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

    public async Task UpdateAsync(
        MarketplaceListing listing,
        CancellationToken cancellationToken = default)
    {
        _context.MarketplaceListings.Update(listing);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
