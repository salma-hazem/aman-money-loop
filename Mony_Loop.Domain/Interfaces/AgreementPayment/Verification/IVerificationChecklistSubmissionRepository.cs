using Mony_Loop.Domain.Entities.Verification;

namespace Mony_Loop.Domain.Interfaces.Verification
{
    public interface IVerificationChecklistSubmissionRepository
    {
        Task<VerificationChecklistSubmission?> GetByIdAsync(Guid verificationChecklistSubmissionId, CancellationToken cancellationToken = default);

        Task<VerificationChecklistSubmission?> GetByVerificationScheduleIdAsync(Guid verificationScheduleId, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<VerificationChecklistSubmission>> GetBySubmittedByUserIdAsync(Guid submittedByUserId, CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(Guid verificationChecklistSubmissionId, CancellationToken cancellationToken = default);

        Task AddAsync(VerificationChecklistSubmission entity, CancellationToken cancellationToken = default);

        Task UpdateByIdAsync(Guid verificationChecklistSubmissionId, VerificationChecklistSubmission entity, CancellationToken cancellationToken = default);

        Task DeleteByIdAsync(Guid verificationChecklistSubmissionId, CancellationToken cancellationToken = default);
    }
}
