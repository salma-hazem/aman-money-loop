using Microsoft.EntityFrameworkCore;
using MonyLoop.Domain.Entities.UserAuth;
using MonyLoop.Domain.Interfaces.UserAuth;
using MonyLoop.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Infrastructure.Repositories.UserAuth
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly MonyLoopDbContext _dbContext;

        public RefreshTokenRepository(MonyLoopDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(RefreshToken token, CancellationToken ct = default)
        {
            await _dbContext.RefreshTokens.AddAsync(token, ct);
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default)
        {
            return await _dbContext.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Token == token, ct);
        }

        public async Task RevokeAllActiveAsync(Guid userId, CancellationToken ct = default)
        {
            var revokedAt = DateTime.UtcNow;
            var activeTokens = await _dbContext.RefreshTokens
                .Where(token => token.UserId == userId && !token.IsRevoked)
                .ToListAsync(ct);

            foreach (var token in activeTokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = revokedAt;
            }
        }
    }
}
