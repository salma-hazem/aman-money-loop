using Microsoft.EntityFrameworkCore;
using Mony_Loop.Domain.Entities.Verification;
using Mony_Loop.Domain.Interfaces.Verification;
using Mony_Loop.Infrastructure.Data;

namespace Mony_Loop.Infrastructure.Repositories.Verification
{
    public class VerificationChecklistSubmissionRepository : IVerificationChecklistSubmissionRepository
    {
        private readonly MonyLoopDbContext _context;

        public VerificationChecklistSubmissionRepository(MonyLoopDbContext context)
        {
            _context = context;
        }

        public async Task<VerificationChecklistSubmission?> GetByIdAsync(Guid verificationChecklistSubmissionId, CancellationToken cancellationToken = default)
        {
            return await _context.VerificationChecklistSubmissions
                .FirstOrDefaultAsync(x => x.VerificationChecklistSubmissionId == verificationChecklistSubmissionId, cancellationToken);
        }

        public async Task<VerificationChecklistSubmission?> GetByVerificationScheduleIdAsync(Guid verificationScheduleId, CancellationToken cancellationToken = default)
        {
            return await _context.VerificationChecklistSubmissions
                .FirstOrDefaultAsync(x => x.VerificationScheduleId == verificationScheduleId, cancellationToken);
        }

        public async Task<IReadOnlyList<VerificationChecklistSubmission>> GetBySubmittedByUserIdAsync(Guid submittedByUserId, CancellationToken cancellationToken = default)
        {
            return await _context.VerificationChecklistSubmissions
                .Where(x => x.SubmittedByUserId == submittedByUserId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsAsync(Guid verificationChecklistSubmissionId, CancellationToken cancellationToken = default)
        {
            return await _context.VerificationChecklistSubmissions
                .AnyAsync(x => x.VerificationChecklistSubmissionId == verificationChecklistSubmissionId, cancellationToken);
        }

        public async Task AddAsync(VerificationChecklistSubmission entity, CancellationToken cancellationToken = default)
        {
            await _context.VerificationChecklistSubmissions
                .AddAsync(entity, cancellationToken);
        }

        public async Task UpdateByIdAsync(Guid verificationChecklistSubmissionId, VerificationChecklistSubmission entity, CancellationToken cancellationToken = default)
        {
            var existingEntity = await GetByIdAsync(verificationChecklistSubmissionId, cancellationToken);
            if (existingEntity == null)
            {
                return;
            }

            existingEntity.VerificationScheduleId = entity.VerificationScheduleId;
            existingEntity.SubmittedByUserId = entity.SubmittedByUserId;
            existingEntity.CompositeScore = entity.CompositeScore;
            existingEntity.OverallComments = entity.OverallComments;
            existingEntity.SubmittedAt = entity.SubmittedAt;

            _context.VerificationChecklistSubmissions.Update(existingEntity);
        }

        public async Task DeleteByIdAsync(Guid verificationChecklistSubmissionId, CancellationToken cancellationToken = default)
        {
            var existingEntity = await GetByIdAsync(verificationChecklistSubmissionId, cancellationToken);
            if (existingEntity != null)
            {
                _context.VerificationChecklistSubmissions.Remove(existingEntity);
            }
        }
    }
}
