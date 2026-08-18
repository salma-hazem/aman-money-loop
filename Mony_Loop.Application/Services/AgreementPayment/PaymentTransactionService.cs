using AutoMapper;
using Mony_Loop.Application.DTOs.AgreementPayment.PaymentTransaction;
using Mony_Loop.Application.ServicesAbstractions.AgreementPayment;
using Mony_Loop.Domain.Constants.Agreement___Payment;
using Mony_Loop.Domain.Interfaces.AgreementPayment;

namespace Mony_Loop.Application.Services.AgreementPayment
{
    public class PaymentTransactionService : IPaymentTransactionService
    {
        private readonly IPaymentTransactionRepository _paymentTransactionRepository;
        private readonly IMapper _mapper;

        public PaymentTransactionService(
            IPaymentTransactionRepository paymentTransactionRepository,
            IMapper mapper)
        {
            _paymentTransactionRepository = paymentTransactionRepository;
            _mapper = mapper;
        }

        public async Task<PaymentsOverviewResponse> GetPaymentsByMemberLedgerAsync(
            Guid memberLedgerId)
        {
            var transactions =
                await _paymentTransactionRepository
                    .GetByMemberLedgerIdAsync(memberLedgerId);

            var successfulPayIns = transactions
                .Where(x =>
                    x.TransactionType == PaymentTransactionType.PayIn &&
                    x.TransactionStatus == PaymentTransactionStatus.Successful)
                .ToList();

            var response = new PaymentsOverviewResponse
            {
                MemberLedgerId = memberLedgerId,

                TotalPaid = successfulPayIns.Sum(x => x.Amount),

                PaidContributionsCount = successfulPayIns.Count,

                Transactions =
                    _mapper.Map<List<PaymentTransactionResponse>>(transactions),

                // These require data outside PaymentTransaction.
                NextContributionAmount = null,
                NextContributionDueDate = null,
                PayoutSlot = null,
                PayoutStatus = null
            };

            return response;
        }
        public Task<PaymentTransactionResponse> RecordPayInAsync(
        RecordPayInRequest request)
            {
                throw new NotImplementedException();
            }

        public async Task<PaymentReceiptResponse?> GetReceiptAsync(
        Guid transactionId)
        {
            var transaction =
                await _paymentTransactionRepository.GetByIdAsync(transactionId);

            if (transaction is null)
                return null;

            if (transaction.TransactionStatus != PaymentTransactionStatus.Successful)
            {
                throw new InvalidOperationException(
                    "A receipt is only available for a successful transaction.");
            }

            return _mapper.Map<PaymentReceiptResponse>(transaction);
            
        }
    }
}