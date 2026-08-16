using Microsoft.EntityFrameworkCore;
using MonyLoop.Domain.Entities.Verification;
using MonyLoop.Domain.Interfaces.Verification;
using MonyLoop.Infrastructure.Data;

namespace MonyLoop.Infrastructure.Repositories.Verification
{
    public class VerificationCriterionRatingRepository : IVerificationCriterionRatingRepository
    {
        private readonly MonyLoopDbContext _context;

        public VerificationCriterionRatingRepository(MonyLoopDbContext context)
        {
            _context = context;
        }

        public async Task<VerificationCriterionRating?> GetByIdAsync(Guid verificationCriterionRatingId, CancellationToken cancellationToken = default)
        {
            return await _context.VerificationCriterionRatings
                .FirstOrDefaultAsync(x => x.VerificationCriterionRatingId == verificationCriterionRatingId, cancellationToken);
        }

        public async Task<IReadOnlyList<VerificationCriterionRating>> GetBySubmissionIdAsync(Guid verificationChecklistSubmissionId, CancellationToken cancellationToken = default)
        {
            return await _context.VerificationCriterionRatings
                .Where(x => x.VerificationChecklistSubmissionId == verificationChecklistSubmissionId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsAsync(Guid verificationCriterionRatingId, CancellationToken cancellationToken = default)
        {
            return await _context.VerificationCriterionRatings
                .AnyAsync(x => x.VerificationCriterionRatingId == verificationCriterionRatingId, cancellationToken);
        }

        public async Task AddAsync(VerificationCriterionRating entity, CancellationToken cancellationToken = default)
        {
            await _context.VerificationCriterionRatings.AddAsync(entity, cancellationToken);
        }

        public async Task UpdateByIdAsync(Guid verificationCriterionRatingId, VerificationCriterionRating entity, CancellationToken cancellationToken = default)
        {
            var existingEntity = await GetByIdAsync(verificationCriterionRatingId, cancellationToken);
            if (existingEntity == null)
            {
                return;
            }

            existingEntity.VerificationChecklistSubmissionId = entity.VerificationChecklistSubmissionId;
            existingEntity.VerificationCriterionId = entity.VerificationCriterionId;
            existingEntity.Rating = entity.Rating;
            existingEntity.Comments = entity.Comments;

            _context.VerificationCriterionRatings.Update(existingEntity);
        }

        public async Task DeleteByIdAsync(Guid verificationCriterionRatingId, CancellationToken cancellationToken = default)
        {
            var existingEntity = await GetByIdAsync(verificationCriterionRatingId, cancellationToken);
            if (existingEntity != null)
            {
                _context.VerificationCriterionRatings.Remove(existingEntity);
            }
        }
    }
}
