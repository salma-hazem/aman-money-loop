using Mony_Loop.Infrastructure.Data;
using Mony_Loop.Domain.Interfaces;

namespace Mony_Loop.Infrastructure.Repositories;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly MonyLoopDbContext _context;

    public UnitOfWork(MonyLoopDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
