using Microsoft.EntityFrameworkCore;
using Mony_Loop.Domain.Entities.Verification;
using Mony_Loop.Domain.Interfaces.Verification;
using Mony_Loop.Infrastructure.Data;

namespace Mony_Loop.Infrastructure.Repositories.Verification
{
    public class VerificationRoundRepository : IVerificationRoundRepository
    {
        private readonly MonyLoopDbContext _context;

        public VerificationRoundRepository(MonyLoopDbContext context)
        {
            _context = context;
        }

        public async Task<VerificationRound?> GetVerificationRoundByIdAsync(Guid VerificationRoundId, CancellationToken cancellationToken = default)
        {
            return await _context.VerificationRounds
                .FirstOrDefaultAsync(x => x.VerificationRoundId == VerificationRoundId, cancellationToken);
        }

        public async Task<IReadOnlyList<VerificationRound>> GetRoundsByCircleIdAsync(Guid circleId, CancellationToken cancellationToken = default)
        {
            return await _context.VerificationRounds
                .Where(x => x.CircleId == circleId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsAsync(Guid VerificationRoundId, CancellationToken cancellationToken = default)
        {
            return await _context.VerificationRounds
                .AnyAsync(x => x.VerificationRoundId == VerificationRoundId, cancellationToken);
        }

        public async Task AddAsync(VerificationRound entity, CancellationToken cancellationToken = default)
        {
            await _context.VerificationRounds.AddAsync(entity, cancellationToken);
        }

        public async Task UpdateByIdAsync(Guid verificationRoundId, VerificationRound entity, CancellationToken cancellationToken = default)
        {
            var existingEntity = await GetVerificationRoundByIdAsync(verificationRoundId, cancellationToken);
            if (existingEntity == null)
            {
                return;
            }

            _context.Entry(existingEntity).CurrentValues.SetValues(entity);

            _context.VerificationRounds.Update(existingEntity);
        }

        public async Task DeleteByIdAsync(Guid VerificationRoundId, CancellationToken cancellationToken = default)
        {
            var existingEntity = await GetVerificationRoundByIdAsync(VerificationRoundId, cancellationToken);
            if (existingEntity != null)
            {
                _context.VerificationRounds.Remove(existingEntity);
            }
        }
    }
}
