using System.Text.Json.Serialization;
using MonyLoop.Domain.Constants;

namespace MonyLoop.Application.DTOs.CircleRequestManagement;

public sealed class CircleResponseDto
{
    public Guid CircleId { get; set; }
    public Guid RequestId { get; set; }
    public string CircleTitle { get; set; } = string.Empty;
    public int ApprovedSlots { get; set; }
    public int FilledCount { get; set; }
    public decimal Amount { get; set; }
    public int Duration { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CircleStatus Status { get; set; }

    public MarketplaceListingResponseDto? MarketplaceListing { get; set; }
}
