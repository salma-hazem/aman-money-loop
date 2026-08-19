using Microsoft.EntityFrameworkCore;
using MonyLoop.Domain.Entities.Verification;
using MonyLoop.Domain.Interfaces.Verification;
using MonyLoop.Infrastructure.Data;

namespace MonyLoop.Infrastructure.Repositories.Verification
{
    public class VerificationScheduleRepository : IVerificationScheduleRepository
    {
        private readonly MonyLoopDbContext _context;

        public VerificationScheduleRepository(MonyLoopDbContext context)
        {
            _context = context;
        }

        public async Task<VerificationSchedule?> GetByIdAsync(Guid verificationScheduleId, CancellationToken cancellationToken = default)
        {
            return await _context.VerificationSchedules
                .FirstOrDefaultAsync(x => x.VerificationScheduleId == verificationScheduleId, cancellationToken);
        }

        public async Task<IReadOnlyList<VerificationSchedule>> GetByApplicationIdAsync(Guid applicationId, CancellationToken cancellationToken = default)
        {
            return await _context.VerificationSchedules
                .Where(x => x.ApplicationId == applicationId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<VerificationSchedule>> GetByVerificationRoundIdAsync(Guid verificationRoundId, CancellationToken cancellationToken = default)
        {
            return await _context.VerificationSchedules
                .Where(x => x.VerificationRoundId == verificationRoundId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsAsync(Guid verificationScheduleId, CancellationToken cancellationToken = default)
        {
            return await _context.VerificationSchedules
                .AnyAsync(x => x.VerificationScheduleId == verificationScheduleId, cancellationToken);
        }

        public async Task AddAsync(VerificationSchedule entity, CancellationToken cancellationToken = default)
        {
            await _context.VerificationSchedules.AddAsync(entity, cancellationToken);
        }

        public async Task UpdateByIdAsync(Guid verificationScheduleId, VerificationSchedule entity, CancellationToken cancellationToken = default)
        {
            var existingEntity = await GetByIdAsync(verificationScheduleId, cancellationToken);
            if (existingEntity == null)
            {
                return;
            }

            _context.Entry(existingEntity).CurrentValues.SetValues(entity);

            // Explicitly mark entity state as Modified in EF Core
            _context.VerificationSchedules.Update(existingEntity);
        }

        public async Task DeleteByIdAsync(Guid verificationScheduleId, CancellationToken cancellationToken = default)
        {
            var existingEntity = await GetByIdAsync(verificationScheduleId, cancellationToken);
            if (existingEntity != null)
            {
                // Explicitly mark entity state as Deleted in EF Core
                _context.VerificationSchedules.Remove(existingEntity);
            }
        }
    }
}
