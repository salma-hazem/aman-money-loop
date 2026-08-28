using Microsoft.EntityFrameworkCore;
using MonyLoop.Domain.Entities.Verification;
using MonyLoop.Domain.Interfaces.Verification;
using MonyLoop.Infrastructure.Data;

namespace MonyLoop.Infrastructure.Repositories.Verification
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

        public async Task UpdateByIdAsync(Guid verificationRoundId, VerificationRound updatedEntity, CancellationToken cancellationToken = default)
        {
            // 1. Fetch the round along with its child criteria collection
            var existingEntity = await _context.VerificationRounds
                .Include(r => r.Criteria)
                .FirstOrDefaultAsync(r => r.VerificationRoundId == verificationRoundId, cancellationToken);

            if (existingEntity == null) return;

            // 2. Update scalar properties on the VerificationRound itself
            _context.Entry(existingEntity).CurrentValues.SetValues(updatedEntity);

            // 3. Delete criteria that were removed in the update
            foreach (var existingCriterion in existingEntity.Criteria.ToList())
            {
                if (!updatedEntity.Criteria.Any(c => c.VerificationCriterionId == existingCriterion.VerificationCriterionId))
                {
                    _context.VerificationCriteria.Remove(existingCriterion);
                }
            }

            // 4. Update existing criteria or add new ones
            foreach (var newCriterion in updatedEntity.Criteria)
            {
                var existingCriterion = existingEntity.Criteria
                    .FirstOrDefault(c => c.VerificationCriterionId == newCriterion.VerificationCriterionId && c.VerificationCriterionId != Guid.Empty);

                if (existingCriterion != null)
                {
                    _context.Entry(existingCriterion).CurrentValues.SetValues(newCriterion);
                }
                else
                {
                    existingEntity.Criteria.Add(newCriterion);
                }
            }
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
