using MonyLoop.Domain.Entities.CircleRequestManagement;

namespace MonyLoop.Domain.Interfaces.CircleRequestManagement;

public interface IMarketplaceListingRepository
{
    Task<MarketplaceListing?> GetByIdAsync(
        Guid listingId,
        CancellationToken cancellationToken = default);

    Task<MarketplaceListing?> GetByCircleIdAsync(
        Guid circleId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MarketplaceListing>> GetActiveAsync(
        CancellationToken cancellationToken = default);

    Task AddAsync(
        MarketplaceListing listing,
        CancellationToken cancellationToken = default);
}
