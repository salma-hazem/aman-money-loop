using Microsoft.EntityFrameworkCore;
using MonyLoop.Domain.Constants;
using MonyLoop.Domain.Entities.CircleRequestManagement;
using MonyLoop.Domain.Interfaces.CircleRequestManagement;
using MonyLoop.Infrastructure.Data;

namespace MonyLoop.Infrastructure.Repositories.CircleRequestManagement;

public sealed class CircleRepository : ICircleRepository
{
    private readonly MonyLoopDbContext _context;

    public CircleRepository(MonyLoopDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Circle>> GetAllAsync(
        CircleStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Circle> query = _context.Circles
            .AsNoTracking()
            .Include(circle => circle.CircleRequest)
            .Include(circle => circle.MarketplaceListing);

        if (status.HasValue)
        {
            query = query.Where(circle => circle.Status == status.Value);
        }

        return await query
            .OrderByDescending(circle => circle.CircleRequest!.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<Circle?> GetByIdAsync(
        Guid circleId,
        CancellationToken cancellationToken = default)
    {
        return _context.Circles.FirstOrDefaultAsync(
            circle => circle.CircleId == circleId,
            cancellationToken);
    }

    public Task<Circle?> GetDetailsByIdAsync(
        Guid circleId,
        CancellationToken cancellationToken = default)
    {
        return _context.Circles
            .AsNoTracking()
            .Include(circle => circle.CircleRequest)
            .Include(circle => circle.MarketplaceListing)
            .Include(circle => circle.CircleSlots)
            .FirstOrDefaultAsync(
                circle => circle.CircleId == circleId,
                cancellationToken);
    }

    public Task<Circle?> GetByRequestIdAsync(
        Guid requestId,
        CancellationToken cancellationToken = default)
    {
        return _context.Circles.FirstOrDefaultAsync(
            circle => circle.RequestId == requestId,
            cancellationToken);
    }

    public Task<bool> ExistsForRequestAsync(
        Guid requestId,
        CancellationToken cancellationToken = default)
    {
        return _context.Circles.AnyAsync(
            circle => circle.RequestId == requestId,
            cancellationToken);
    }

    public Task AddAsync(
        Circle circle,
        CancellationToken cancellationToken = default)
    {
        return _context.Circles
            .AddAsync(circle, cancellationToken)
            .AsTask();
    }
}
