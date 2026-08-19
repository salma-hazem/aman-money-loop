using Microsoft.EntityFrameworkCore;
using MonyLoop.Domain.Constants.UserAuth;
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
    public class OTPTokenRepository : IOTPTokenRepository
    {
        private readonly MonyLoopDbContext _dbContext;

        public OTPTokenRepository(MonyLoopDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task AddAsync(OTPToken otpToken, CancellationToken ct = default)
        {
            await _dbContext.OTPTokens.AddAsync(otpToken, ct);
        }

        public async Task<OTPToken?> GetLatestActiveAsync(Guid userId, OTPPurpose purpose, CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;
            return await _dbContext.OTPTokens
                .Where(o => o.UserId == userId
                            && o.Purpose == purpose
                            && !o.IsUsed)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync(ct);
        }

        public async Task InvalidateExistingTokensAsync(Guid userId, OTPPurpose purpose, CancellationToken ct = default)
        {
            var activeTokens = await _dbContext.OTPTokens
                .Where(o => o.UserId == userId && o.Purpose == purpose && !o.IsUsed)
                .ToListAsync(ct);

            foreach (var token in activeTokens)
            {
                token.IsUsed = true;
            }
        }


    }
}
