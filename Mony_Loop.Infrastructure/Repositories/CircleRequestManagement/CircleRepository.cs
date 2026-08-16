using Microsoft.EntityFrameworkCore;
using Mony_Loop.Domain.Entities.CircleRequestManagement;
using Mony_Loop.Domain.Interfaces.CircleRequestManagement;
using Mony_Loop.Infrastructure.Data;

namespace Mony_Loop.Infrastructure.Repositories.CircleRequestManagement;

public sealed class CircleRepository : ICircleRepository
{
    private readonly MonyLoopDbContext _context;

    public CircleRepository(MonyLoopDbContext context)
    {
        _context = context;
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
