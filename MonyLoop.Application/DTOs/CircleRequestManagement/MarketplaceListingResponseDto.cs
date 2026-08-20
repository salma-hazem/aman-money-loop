using System.Text.Json.Serialization;
using MonyLoop.Domain.Constants;

namespace MonyLoop.Application.DTOs.CircleRequestManagement;

public sealed class MarketplaceListingResponseDto
{
    public Guid ListingId { get; set; }
    public Guid CircleId { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public MarketplaceListingStatus ListingStatus { get; set; }
}
