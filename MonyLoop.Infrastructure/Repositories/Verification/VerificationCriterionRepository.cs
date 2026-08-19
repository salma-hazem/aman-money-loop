using Microsoft.EntityFrameworkCore;
using MonyLoop.Domain.Entities.Verification;
using MonyLoop.Domain.Interfaces.Verification;
using MonyLoop.Infrastructure.Data;

namespace MonyLoop.Infrastructure.Repositories.Verification
{
    public class VerificationCriterionRepository : IVerificationCriterionRepository
    {
        private readonly MonyLoopDbContext _context;

        public VerificationCriterionRepository(MonyLoopDbContext context)
        {
            _context = context;
        }

        public async Task<VerificationCriterion?> GetByIdAsync(Guid verificationCriterionId, CancellationToken cancellationToken = default)
        {
            return await _context.VerificationCriteria
                .FirstOrDefaultAsync(x => x.VerificationCriterionId == verificationCriterionId, cancellationToken);
        }

        public async Task<IReadOnlyList<VerificationCriterion>> GetByVerificationRoundIdAsync(Guid verificationRoundId, CancellationToken cancellationToken = default)
        {
            return await _context.VerificationCriteria
                .Where(x => x.VerificationRoundId == verificationRoundId)
                .OrderBy(x => x.DisplayOrder)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsAsync(Guid verificationCriterionId, CancellationToken cancellationToken = default)
        {
            return await _context.VerificationCriteria
                .AnyAsync(x => x.VerificationCriterionId == verificationCriterionId, cancellationToken);
        }

        public async Task AddAsync(VerificationCriterion entity, CancellationToken cancellationToken = default)
        {
            await _context.VerificationCriteria.AddAsync(entity, cancellationToken);
        }

        public async Task UpdateByIdAsync(Guid verificationCriterionId, VerificationCriterion entity, CancellationToken cancellationToken = default)
        {
            var existingEntity = await GetByIdAsync(verificationCriterionId, cancellationToken);
            if (existingEntity == null)
            {
                return;
            }

            existingEntity.VerificationRoundId = entity.VerificationRoundId;
            existingEntity.CriterionName = entity.CriterionName;
            existingEntity.Weight = entity.Weight;
            existingEntity.DisplayOrder = entity.DisplayOrder;
            existingEntity.IsActive = entity.IsActive;

            _context.VerificationCriteria.Update(existingEntity);
        }

        public async Task DeleteByIdAsync(Guid verificationCriterionId, CancellationToken cancellationToken = default)
        {
            var existingEntity = await GetByIdAsync(verificationCriterionId, cancellationToken);
            if (existingEntity != null)
            {
                
    _context.VerificationCriteria.Remove(existingEntity);
            }
        }
    }
}
