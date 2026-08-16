using Microsoft.EntityFrameworkCore;
using Mony_Loop.Domain.Entities.CircleRequestManagement;
using Mony_Loop.Domain.Interfaces.CircleRequestManagement;
using Mony_Loop.Infrastructure.Data;

namespace Mony_Loop.Infrastructure.Repositories.CircleRequestManagement;

public sealed class AuditLogRepository : IAuditLogRepository
{
    private readonly MonyLoopDbContext _context;

    public AuditLogRepository(MonyLoopDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<AuditLog>> GetByEntityAsync(
        string entityType,
        Guid entityId,
        CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs
            .AsNoTracking()
            .Where(auditLog =>
                auditLog.EntityType == entityType &&
                auditLog.EntityId == entityId)
            .OrderByDescending(auditLog => auditLog.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AuditLog>> GetByPerformedUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs
            .AsNoTracking()
            .Where(auditLog => auditLog.PerformedByUserId == userId)
            .OrderByDescending(auditLog => auditLog.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task AddAsync(
        AuditLog auditLog,
        CancellationToken cancellationToken = default)
    {
        return _context.AuditLogs
            .AddAsync(auditLog, cancellationToken)
            .AsTask();
    }
}
