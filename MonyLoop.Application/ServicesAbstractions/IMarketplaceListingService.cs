using MonyLoop.Application.Common;
using MonyLoop.Application.DTOs.CircleRequestManagement;

namespace MonyLoop.Application.ServicesAbstractions;

public interface IMarketplaceListingService
{
    Task<Result<IReadOnlyList<MarketplaceListingSummaryDto>>> GetActiveListingsAsync(MarketplaceListingQueryDto query);

    Task<Result<MarketplaceListingDetailDto>> GetByIdAsync(Guid listingId);
}