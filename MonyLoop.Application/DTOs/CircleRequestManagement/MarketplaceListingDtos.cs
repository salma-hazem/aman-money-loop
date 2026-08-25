using System.Text.Json.Serialization;
using MonyLoop.Domain.Constants;

namespace MonyLoop.Application.DTOs.CircleRequestManagement;

// Screen 1 - Marketplace browse listing card
public sealed class MarketplaceListingSummaryDto
{
    public Guid ListingId { get; set; }
    public Guid CircleId { get; set; }
    public string Title { get; set; } = string.Empty;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public MarketplaceListingStatus ListingStatus { get; set; }

    public decimal MonthlyContribution { get; set; }
    public int DurationMonths { get; set; }
    public int TotalSlots { get; set; }
    public int AvailableSlots { get; set; }
}

// Screen 2 - Circle Details
public sealed class MarketplaceListingDetailDto
{
    public Guid ListingId { get; set; }
    public Guid CircleId { get; set; }
    public string Title { get; set; } = string.Empty;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public MarketplaceListingStatus ListingStatus { get; set; }

    public decimal MonthlyContribution { get; set; }
    public int DurationMonths { get; set; }
    public int TotalSlots { get; set; }
    public int FilledSlots { get; set; }
    public int AvailableSlots { get; set; }
}
public sealed class MarketplaceListingQueryDto
{
    public string? Search { get; set; }
    public decimal? MinContribution { get; set; }
    public decimal? MaxContribution { get; set; }
    public int? MinDuration { get; set; }
    public int? MaxDuration { get; set; }
    public int? MinAvailableSlots { get; set; }
}