using System.ComponentModel.DataAnnotations;

namespace MonyLoop.Application.DTOs.CircleRequestManagement;

public sealed class DecisionReasonDto
{
    [Required]
    [StringLength(1000)]
    public string Reason { get; set; } = string.Empty;
}
