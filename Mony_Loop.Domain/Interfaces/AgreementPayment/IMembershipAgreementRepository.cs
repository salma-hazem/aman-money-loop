using Mony_Loop.Domain.Entities.Agreement___Payment;

namespace Mony_Loop.Domain.Interfaces.AgreementPayment
{
    public interface IMembershipAgreementRepository
    {
        Task<MembershipAgreement?> GetByIdAsync(
            Guid agreementId,
            CancellationToken cancellationToken = default);

        Task<MembershipAgreement?> GetByMembershipApplicationIdAsync(
            Guid membershipApplicationId,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsForMembershipApplicationAsync(
            Guid membershipApplicationId,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            MembershipAgreement agreement,
            CancellationToken cancellationToken = default);

        void Update(MembershipAgreement agreement);
    }
}