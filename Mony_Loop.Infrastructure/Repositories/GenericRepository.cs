using Microsoft.EntityFrameworkCore;
using Mony_Loop.Infrastructure.Data;
using MonyLoop.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mony_Loop.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly MonyLoopDbContext _dbcontext;

        public GenericRepository(MonyLoopDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task AddAsync(T entity, CancellationToken ct = default)
          => await _dbcontext.Set<T>().AddAsync(entity, ct);

        public void Delete(T entity)
        => _dbcontext.Set<T>().Remove(entity);

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default)
        => await _dbcontext.Set<T>().ToListAsync(ct);

        public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _dbcontext.Set<T>().FindAsync(new object[] { id }, ct);


        public void Update(T entity)
        => _dbcontext.Set<T>().Update(entity);
    }
}
