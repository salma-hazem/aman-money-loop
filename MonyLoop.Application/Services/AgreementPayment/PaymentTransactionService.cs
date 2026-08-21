using AutoMapper;
using MonyLoop.Application.DTOs.AgreementPayment.PaymentTransaction;
using MonyLoop.Application.ServicesAbstractions.AgreementPayment;
using MonyLoop.Domain.Constants.Agreement___Payment;
using MonyLoop.Domain.Entities.Agreement___Payment;
using MonyLoop.Domain.Interfaces;
using MonyLoop.Domain.Interfaces.AgreementPayment;
using MonyLoop.Domain.Interfaces.OnboardingMemberLedger;

namespace MonyLoop.Application.Services.AgreementPayment
{
    public class PaymentTransactionService : IPaymentTransactionService
    {
        private readonly IPaymentTransactionRepository _paymentTransactionRepository;
        private readonly IMemberLedgerRepository _memberLedgerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PaymentTransactionService(
        IPaymentTransactionRepository paymentTransactionRepository,
        IMemberLedgerRepository memberLedgerRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
            {
                _paymentTransactionRepository = paymentTransactionRepository;
                _memberLedgerRepository = memberLedgerRepository;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }

        // GET PAYMENTS OVERVIEW
        public async Task<PaymentsOverviewResponse> GetPaymentsByMemberLedgerAsync(
            Guid memberLedgerId)
        {
            var transactions =
                await _paymentTransactionRepository
                    .GetByMemberLedgerIdAsync(memberLedgerId);
            var memberLedger =
                await _memberLedgerRepository
                    .GetByIdWithSlotAsync(memberLedgerId);
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
                Transactions = _mapper.Map<List<PaymentTransactionResponse>>(transactions),
                PayoutSlot = memberLedger?.CircleSlot?.SlotNumber,
                // TODO:
                // These values still require additional business/cross-module data.
                NextContributionAmount = null,
                NextContributionDueDate = null,
                PayoutStatus = null
            };

            return response;
        }

        // RECORD PAY-IN
        public async Task<PaymentTransactionResponse> RecordPayInAsync(RecordPayInRequest request)
        {
            // Validate amount
            if (request.Amount <= 0)
            {
                throw new ArgumentException(
                    "Payment amount must be greater than zero.");
            }
            // Retrieve Member Ledger including its CircleSlot
            var memberLedger = await _memberLedgerRepository.GetByIdWithSlotAsync(request.MemberLedgerId);
            if (memberLedger is null)
            {
                throw new KeyNotFoundException(
                    "Member ledger was not found.");
            }
            // Ledger must already belong to a circle slot
            if (memberLedger.CircleSlot is null)
            {
                throw new InvalidOperationException(
                    "The member ledger is not assigned to a circle slot.");
            }
            // Prevent duplicate transaction references
            if (!string.IsNullOrWhiteSpace(request.TransactionReference))
            {
                var referenceExists =
                    await _paymentTransactionRepository
                        .ExistsByTransactionReferenceAsync(
                            request.TransactionReference);

                if (referenceExists)
                {
                    throw new InvalidOperationException(
                        "A transaction with this reference already exists.");
                }
            }

            var now = DateTime.UtcNow;
            var paymentTransaction = new PaymentTransaction
            {
                PaymentTransactionId = Guid.NewGuid(),
                MemberLedgerId = memberLedger.MemberLedgerId,
                // Derived from MemberLedger -> CircleSlot
                CircleId = memberLedger.CircleSlot.CircleId,
                // TODO:
                // Replace this with the authenticated user's ID.
                RecordedByUserId = Guid.Empty,
                TransactionType = PaymentTransactionType.PayIn,
                PaymentMethod = request.PaymentMethod,
                TransactionStatus = PaymentTransactionStatus.Successful,
                Amount = request.Amount,
                TransactionReference = request.TransactionReference,
                // FR21 requires a digital receipt for each transaction.
                ReceiptNumber = GenerateReceiptNumber(),
                TransactionDate = now,
                CreatedAt = now
            };

            await _paymentTransactionRepository.AddAsync(paymentTransaction);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<PaymentTransactionResponse>(paymentTransaction);
        }
        public async Task<PaymentTransactionResponse> RecordPayOutAsync(RecordPayOutRequest request)
        {
            if (request.Amount <= 0)
            {
                throw new ArgumentException(
                    "Payout amount must be greater than zero.");
            }
            var memberLedger = await _memberLedgerRepository.GetByIdWithSlotAsync(request.MemberLedgerId);

            if (memberLedger is null)
            {
                throw new KeyNotFoundException(
                    "Member ledger was not found.");
            }

            if (memberLedger.CircleSlot is null)
            {
                throw new InvalidOperationException(
                    "The member ledger is not assigned to a circle slot.");
            }

            if (!string.IsNullOrWhiteSpace(request.TransactionReference))
            {
                var referenceExists =
                    await _paymentTransactionRepository
                        .ExistsByTransactionReferenceAsync(
                            request.TransactionReference);

                if (referenceExists)
                {
                    throw new InvalidOperationException(
                        "A transaction with this reference already exists.");
                }
            }

            var now = DateTime.UtcNow;
            var paymentTransaction = new PaymentTransaction
            {
                PaymentTransactionId = Guid.NewGuid(),
                MemberLedgerId = memberLedger.MemberLedgerId,
                CircleId = memberLedger.CircleSlot.CircleId,
                // TODO:
                // Replace with authenticated user's ID.
                RecordedByUserId = Guid.Empty,
                TransactionType = PaymentTransactionType.PayOut,
                PaymentMethod = request.PaymentMethod,
                TransactionStatus = PaymentTransactionStatus.Successful,
                Amount = request.Amount,
                TransactionReference = request.TransactionReference,
                ReceiptNumber = GenerateReceiptNumber(),
                TransactionDate = now,
                CreatedAt = now
            };

            await _paymentTransactionRepository.AddAsync(paymentTransaction);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<PaymentTransactionResponse>(paymentTransaction);
        }

        // GET RECEIPT
        public async Task<PaymentReceiptResponse?> GetReceiptAsync(Guid transactionId)
        {
            var transaction = await _paymentTransactionRepository.GetByIdAsync(transactionId);
            if (transaction is null)
                return null;
            return _mapper.Map<PaymentReceiptResponse>(transaction);
        }

        // HELPERS
        private static string GenerateReceiptNumber()
        {
            return
                $"RCPT-{DateTime.UtcNow:yyyyMMddHHmmss}-" +
                $"{Guid.NewGuid().ToString("N")[..6].ToUpper()}";
        }
    }
}