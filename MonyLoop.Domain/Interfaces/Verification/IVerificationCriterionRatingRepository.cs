using MonyLoop.Domain.Entities.Verification;

namespace MonyLoop.Domain.Interfaces.Verification
{
    public interface IVerificationCriterionRatingRepository
    {
        Task<VerificationCriterionRating?> GetByIdAsync(Guid verificationCriterionRatingId, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<VerificationCriterionRating>> GetBySubmissionIdAsync(Guid verificationChecklistSubmissionId, CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(Guid verificationCriterionRatingId, CancellationToken cancellationToken = default);

        Task AddAsync(VerificationCriterionRating entity, CancellationToken cancellationToken = default);

        Task UpdateByIdAsync(Guid verificationCriterionRatingId, VerificationCriterionRating entity, CancellationToken cancellationToken = default);

        Task DeleteByIdAsync(Guid verificationCriterionRatingId, CancellationToken cancellationToken = default);
    }
}
