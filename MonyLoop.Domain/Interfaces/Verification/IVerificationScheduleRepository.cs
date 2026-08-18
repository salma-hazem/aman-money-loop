using MonyLoop.Domain.Entities.Verification;

namespace MonyLoop.Domain.Interfaces.Verification
{
    public interface IVerificationScheduleRepository
    {
        Task<VerificationSchedule?> GetByIdAsync(Guid verificationScheduleId, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<VerificationSchedule>> GetByApplicationIdAsync(Guid applicationId, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<VerificationSchedule>> GetByVerificationRoundIdAsync(Guid verificationRoundId, CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(Guid verificationScheduleId, CancellationToken cancellationToken = default);

        Task AddAsync(VerificationSchedule entity, CancellationToken cancellationToken = default);

        Task UpdateByIdAsync(Guid verificationScheduleId, VerificationSchedule entity, CancellationToken cancellationToken = default);


        Task DeleteByIdAsync(Guid verificationScheduleId, CancellationToken cancellationToken = default);
    }
}
