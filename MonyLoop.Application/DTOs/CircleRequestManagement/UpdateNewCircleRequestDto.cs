using System.ComponentModel.DataAnnotations;

namespace MonyLoop.Application.DTOs.CircleRequestManagement;

public sealed class UpdateNewCircleRequestDto
{
    [Required]
    [StringLength(128)]
    public string CircleTitle { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "9999999999999999.99")]
    public decimal ContributionAmount { get; set; }

    [Range(1, int.MaxValue)]
    public int Duration { get; set; }

    [Range(1, int.MaxValue)]
    public int NumberOfSlots { get; set; }

    [Required]
    [StringLength(500)]
    public string ShortJustification { get; set; } = string.Empty;
}
