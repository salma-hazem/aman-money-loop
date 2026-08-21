using System.Text.Json.Serialization;
using MonyLoop.Domain.Constants;

namespace MonyLoop.Application.DTOs.CircleRequestManagement;

public sealed class CircleSlotResponseDto
{
    public Guid CircleSlotId { get; set; }
    public Guid CircleId { get; set; }
    public Guid? MemberLedgerId { get; set; }
    public int SlotNumber { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CircleSlotStatus Status { get; set; }

    public DateTime? VacatedAt { get; set; }
    public DateTime? AssignedAt { get; set; }
}
