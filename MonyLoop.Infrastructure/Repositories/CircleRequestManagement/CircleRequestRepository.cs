using Microsoft.EntityFrameworkCore;
using MonyLoop.Domain.Constants;
using MonyLoop.Domain.Entities.CircleRequestManagement;
using MonyLoop.Domain.Interfaces.CircleRequestManagement;
using MonyLoop.Infrastructure.Data;

namespace MonyLoop.Infrastructure.Repositories.CircleRequestManagement;

public sealed class CircleRequestRepository : ICircleRequestRepository
{
    private readonly MonyLoopDbContext _context;

    public CircleRequestRepository(MonyLoopDbContext context)
    {
        _context = context;
    }

    public Task<CircleRequest?> GetByIdAsync(
        Guid requestId,
        CancellationToken cancellationToken = default)
    {
        return _context.CircleRequests
            .FirstOrDefaultAsync(
                request => request.RequestId == requestId,
                cancellationToken);
    }

    public async Task<IReadOnlyList<CircleRequest>> GetByOrganizerIdAsync(
        Guid organizerId,
        CancellationToken cancellationToken = default)
    {
        return await _context.CircleRequests
            .AsNoTracking()
            .Where(request => request.CreatedByOrganizerId == organizerId)
            .OrderByDescending(request => request.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CircleRequest>> GetByStatusAsync(
        CircleRequestStatus status,
        CancellationToken cancellationToken = default)
    {
        return await _context.CircleRequests
            .AsNoTracking()
            .Where(request => request.RequestStatus == status)
            .OrderBy(request => request.SubmittedAt ?? request.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CircleRequest>> GetReplacementRequestsAsync(
        Guid existingCircleId,
        CancellationToken cancellationToken = default)
    {
        return await _context.CircleRequests
            .AsNoTracking()
            .Where(request => request.ExistingCircleId == existingCircleId)
            .OrderByDescending(request => request.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsAsync(
        Guid requestId,
        CancellationToken cancellationToken = default)
    {
        return _context.CircleRequests.AnyAsync(
            request => request.RequestId == requestId,
            cancellationToken);
    }

    public Task AddAsync(
        CircleRequest request,
        CancellationToken cancellationToken = default)
    {
        return _context.CircleRequests
            .AddAsync(request, cancellationToken)
            .AsTask();
    }
}
