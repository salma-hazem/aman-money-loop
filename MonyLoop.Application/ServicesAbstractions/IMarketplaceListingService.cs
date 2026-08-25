using MonyLoop.Application.Common;
using MonyLoop.Application.DTOs.CircleRequestManagement;

namespace MonyLoop.Application.ServicesAbstractions;

public interface IMarketplaceListingService
{
    Task<Result<IReadOnlyList<MarketplaceListingSummaryDto>>> GetActiveListingsAsync();

    Task<Result<MarketplaceListingDetailDto>> GetByIdAsync(Guid listingId);
}