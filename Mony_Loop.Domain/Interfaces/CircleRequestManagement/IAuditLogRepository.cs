using Mony_Loop.Domain.Entities.CircleRequestManagement;

namespace Mony_Loop.Domain.Interfaces.CircleRequestManagement;

public interface IAuditLogRepository
{
    Task<IReadOnlyList<AuditLog>> GetByEntityAsync(
        string entityType,
        Guid entityId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AuditLog>> GetByPerformedUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        AuditLog auditLog,
        CancellationToken cancellationToken = default);
}
