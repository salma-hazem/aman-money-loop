using Microsoft.EntityFrameworkCore;
using MonyLoop.Domain.Entities.Agreement___Payment;
using MonyLoop.Domain.Interfaces.AgreementPayment;
using MonyLoop.Infrastructure.Data;

namespace MonyLoop.Infrastructure.Repositories.AgreementPayment
{
    public class PaymentTransactionRepository
        : IPaymentTransactionRepository
    {
        private readonly MonyLoopDbContext _context;

        public PaymentTransactionRepository(
            MonyLoopDbContext context)
        {
            _context = context;
        }

        public async Task<PaymentTransaction?> GetByIdAsync(
            Guid paymentTransactionId,
            CancellationToken cancellationToken = default)
        {
            return await _context.PaymentTransactions
                .FirstOrDefaultAsync(
                    x => x.PaymentTransactionId == paymentTransactionId,
                    cancellationToken);
        }

        public async Task<IReadOnlyList<PaymentTransaction>>
            GetByMemberLedgerIdAsync(
                Guid memberLedgerId,
                CancellationToken cancellationToken = default)
        {
            return await _context.PaymentTransactions
                .Where(x => x.MemberLedgerId == memberLedgerId)
                .OrderByDescending(x => x.TransactionDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<PaymentTransaction>>
            GetByCircleIdAsync(
                Guid circleId,
                CancellationToken cancellationToken = default)
        {
            return await _context.PaymentTransactions
                .Where(x => x.CircleId == circleId)
                .OrderByDescending(x => x.TransactionDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsByTransactionReferenceAsync(
            string transactionReference,
            CancellationToken cancellationToken = default)
        {
            return await _context.PaymentTransactions
                .AnyAsync(
                    x => x.TransactionReference == transactionReference,
                    cancellationToken);
        }

        public async Task AddAsync(
            PaymentTransaction paymentTransaction,
            CancellationToken cancellationToken = default)
        {
            await _context.PaymentTransactions
                .AddAsync(paymentTransaction, cancellationToken);
        }

        public void Update(PaymentTransaction paymentTransaction)
        {
            _context.PaymentTransactions.Update(paymentTransaction);
        }
    }
}