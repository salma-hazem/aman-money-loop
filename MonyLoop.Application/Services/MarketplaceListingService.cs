using MonyLoop.Application.Common;
using MonyLoop.Application.DTOs.CircleRequestManagement;
using MonyLoop.Application.ServicesAbstractions;
using MonyLoop.Domain.Constants;
using MonyLoop.Domain.Entities.CircleRequestManagement;
using MonyLoop.Domain.Interfaces.CircleRequestManagement;


namespace MonyLoop.Application.Services;

public class MarketplaceListingService : IMarketplaceListingService
{
    private readonly IMarketplaceListingRepository _repository;

    public MarketplaceListingService(IMarketplaceListingRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IReadOnlyList<MarketplaceListingSummaryDto>>> GetActiveListingsAsync(MarketplaceListingQueryDto query)
    {
        var listings = await _repository.GetActiveAsync();

        var summaries = listings
            .Where(l => l.Circle is not null)
            .Select(ToSummaryDto)
            .Where(s => string.IsNullOrWhiteSpace(query.Search) ||
                s.Title.Contains(query.Search, StringComparison.OrdinalIgnoreCase))
            .Where(s => query.MinContribution is null || s.MonthlyContribution >= query.MinContribution)
            .Where(s => query.MaxContribution is null || s.MonthlyContribution <= query.MaxContribution)
            .Where(s => query.MinDuration is null || s.DurationMonths >= query.MinDuration)
            .Where(s => query.MaxDuration is null || s.DurationMonths <= query.MaxDuration)
            .Where(s => query.MinAvailableSlots is null || s.AvailableSlots >= query.MinAvailableSlots)
            .ToList();

        return Result<IReadOnlyList<MarketplaceListingSummaryDto>>.Ok(summaries);
    }

    public async Task<Result<MarketplaceListingDetailDto>> GetByIdAsync(Guid listingId)
    {
        var listing = await _repository.GetDetailsByIdAsync(listingId);

        if (listing is null || listing.Circle is null)
            return Error.NotFound("MarketplaceListing.NotFound", "Listing not found.");

        return ToDetailDto(listing);
    }
    public async Task<Result<MarketplaceListingDetailDto>> UpdateStatusAsync(Guid listingId, MarketplaceListingStatus status)
    {
        var listing = await _repository.GetDetailsByIdAsync(listingId);

        if (listing is null || listing.Circle is null)
            return Error.NotFound("MarketplaceListing.NotFound", "Listing not found.");

        listing.ListingStatus = status;

        await _repository.UpdateAsync(listing);

        return ToDetailDto(listing);
    }

    private static MarketplaceListingSummaryDto ToSummaryDto(MarketplaceListing listing) => new()
    {
        ListingId = listing.ListingId,
        CircleId = listing.CircleId,
        Title = listing.Circle!.CircleRequest?.CircleTitle ?? "Untitled Circle",
        ListingStatus = listing.ListingStatus,
        MonthlyContribution = listing.Circle.Amount,
        DurationMonths = listing.Circle.Duration,
        TotalSlots = listing.Circle.ApprovedSlots,
        AvailableSlots = listing.Circle.ApprovedSlots - listing.Circle.FilledCount,
    };

    private static MarketplaceListingDetailDto ToDetailDto(MarketplaceListing listing) => new()
    {
        ListingId = listing.ListingId,
        CircleId = listing.CircleId,
        Title = listing.Circle!.CircleRequest?.CircleTitle ?? "Untitled Circle",
        ListingStatus = listing.ListingStatus,
        MonthlyContribution = listing.Circle.Amount,
        DurationMonths = listing.Circle.Duration,
        TotalSlots = listing.Circle.ApprovedSlots,
        FilledSlots = listing.Circle.FilledCount,
        AvailableSlots = listing.Circle.ApprovedSlots - listing.Circle.FilledCount,
    };
}