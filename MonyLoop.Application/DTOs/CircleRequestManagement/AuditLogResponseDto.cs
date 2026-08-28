namespace MonyLoop.Application.DTOs.CircleRequestManagement;

public sealed class AuditLogResponseDto
{
    public Guid AuditLogId { get; set; }
    public Guid? EntityId { get; set; }
    public Guid PerformedByUserId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public string ActionType { get; set; } = string.Empty;
    public string? OldStatus { get; set; }
    public string? NewStatus { get; set; }
    public string? ActionDescription { get; set; }
    public DateTime CreatedAt { get; set; }
}
