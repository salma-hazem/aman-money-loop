using MonyLoop.Domain.Entities.CircleRequestManagement;

namespace MonyLoop.Application.Services.CircleRequestManagement;

internal static class AuditLogFactory
{
    public static AuditLog Create(
        string entityType,
        Guid entityId,
        string actionType,
        Guid actorUserId,
        string? oldStatus,
        string? newStatus,
        string description,
        DateTime createdAt)
    {
        return new AuditLog
        {
            AuditLogId = Guid.NewGuid(),
            EntityType = entityType,
            EntityId = entityId,
            ActionType = actionType,
            PerformedByUserId = actorUserId,
            OldStatus = oldStatus,
            NewStatus = newStatus,
            ActionDescription = description,
            CreatedAt = createdAt
        };
    }
}
