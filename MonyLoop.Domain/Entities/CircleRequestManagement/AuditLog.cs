namespace MonyLoop.Domain.Entities.CircleRequestManagement
{
    public class AuditLog
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

        // public User? PerformedByUser { get; set; }
    }
}
