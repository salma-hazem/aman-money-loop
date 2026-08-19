using MonyLoop.Domain.Entities.Verification;

namespace MonyLoop.Domain.Interfaces.Verification
{
    public interface IVerificationCriterionRepository
    {
        Task<VerificationCriterion?> GetByIdAsync(Guid verificationCriterionId, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<VerificationCriterion>> GetByVerificationRoundIdAsync(Guid verificationRoundId, CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(Guid verificationCriterionId, CancellationToken cancellationToken = default);

        Task AddAsync(VerificationCriterion entity, CancellationToken cancellationToken = default);

        Task UpdateByIdAsync(Guid verificationCriterionId, VerificationCriterion entity, CancellationToken cancellationToken = default);

        Task DeleteByIdAsync(Guid verificationCriterionId, CancellationToken cancellationToken = default);
    }
}
