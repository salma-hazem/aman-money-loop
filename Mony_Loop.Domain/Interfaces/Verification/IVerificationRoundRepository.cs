using Mony_Loop.Domain.Entities.Verification;

namespace Mony_Loop.Domain.Interfaces.Verification
{
    public interface IVerificationRoundRepository
    {
        Task<VerificationRound?> GetVerificationRoundByIdAsync(Guid VerificatonRoundId, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<VerificationRound>> GetRoundsByCircleIdAsync(Guid circleId, CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(Guid VerificationRoundId, CancellationToken cancellationToken = default);

        Task AddAsync(VerificationRound entity, CancellationToken cancellationToken = default);

        Task DeleteByIdAsync(Guid VerificationRoundId, CancellationToken cancellationToken = default);

        Task UpdateByIdAsync(Guid verificationRoundId, VerificationRound entity, CancellationToken cancellationToken = default);
    }
}
