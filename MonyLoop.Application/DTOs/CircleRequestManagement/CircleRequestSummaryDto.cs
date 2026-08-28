using System.Text.Json.Serialization;
using MonyLoop.Domain.Constants;

namespace MonyLoop.Application.DTOs.CircleRequestManagement;

public sealed class CircleRequestSummaryDto
{
    public Guid RequestId { get; set; }
    public string CircleTitle { get; set; } = string.Empty;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CircleType CircleType { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CircleRequestStatus RequestStatus { get; set; }

    public decimal ContributionAmount { get; set; }
    public int Duration { get; set; }
    public int NumberOfSlots { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
}
