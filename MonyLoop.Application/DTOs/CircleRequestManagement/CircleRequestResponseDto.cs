using System.Text.Json.Serialization;
using MonyLoop.Domain.Constants;

namespace MonyLoop.Application.DTOs.CircleRequestManagement;

public sealed class CircleRequestResponseDto
{
    public Guid RequestId { get; set; }
    public Guid? ExistingCircleId { get; set; }
    public Guid CreatedByOrganizerId { get; set; }
    public Guid? ReviewedByAdminId { get; set; }
    public string CircleTitle { get; set; } = string.Empty;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CircleType CircleType { get; set; }

    public decimal ContributionAmount { get; set; }
    public int Duration { get; set; }
    public int NumberOfSlots { get; set; }
    public string? ShortJustification { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CircleRequestStatus RequestStatus { get; set; }

    public int? VacantSlotNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? DecisionReason { get; set; }
}
