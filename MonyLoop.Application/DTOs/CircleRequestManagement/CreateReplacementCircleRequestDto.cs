using System.ComponentModel.DataAnnotations;

namespace MonyLoop.Application.DTOs.CircleRequestManagement;

public sealed class CreateReplacementCircleRequestDto
{
    public Guid ExistingCircleId { get; set; }

    [Range(1, int.MaxValue)]
    public int VacantSlotNumber { get; set; }

    [StringLength(500)]
    public string? ShortJustification { get; set; }
}
