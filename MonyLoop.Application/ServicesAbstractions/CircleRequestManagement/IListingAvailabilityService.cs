using MonyLoop.Application.Common;
using MonyLoop.Application.DTOs.CircleRequestManagement;

namespace MonyLoop.Application.ServicesAbstractions.CircleRequestManagement;

public interface IListingAvailabilityService
{
    Task<Result<MarketplaceListingResponseDto>> GetActiveListingAsync(Guid listingId, CancellationToken cancellationToken = default);
    Task<bool> IsActiveAsync(Guid listingId, CancellationToken cancellationToken = default);
}
