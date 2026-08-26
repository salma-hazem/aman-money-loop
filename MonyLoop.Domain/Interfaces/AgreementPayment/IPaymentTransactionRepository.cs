using MonyLoop.Domain.Entities.Agreement___Payment;

namespace MonyLoop.Domain.Interfaces.AgreementPayment
{
    public interface IPaymentTransactionRepository
    {
        Task<PaymentTransaction?> GetByIdAsync(
            Guid paymentTransactionId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<PaymentTransaction>> GetByMemberLedgerIdAsync(
            Guid memberLedgerId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<PaymentTransaction>> GetByCircleIdAsync(
            Guid circleId,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsByTransactionReferenceAsync(
            string transactionReference,
            CancellationToken cancellationToken = default);

        Task<bool> HasSuccessfulPayoutAsync(
            Guid memberLedgerId,
            CancellationToken cancellationToken = default);
        Task AddAsync(
            PaymentTransaction paymentTransaction,
            CancellationToken cancellationToken = default);

        void Update(PaymentTransaction paymentTransaction);
    }
}