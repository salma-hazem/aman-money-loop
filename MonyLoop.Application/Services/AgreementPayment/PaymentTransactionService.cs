using AutoMapper;
using MonyLoop.Application.DTOs.AgreementPayment.PaymentTransaction;
using MonyLoop.Application.ServicesAbstractions.AgreementPayment;
using MonyLoop.Domain.Constants.Agreement___Payment;
using MonyLoop.Domain.Entities.Agreement___Payment;
using MonyLoop.Domain.Interfaces;
using MonyLoop.Domain.Interfaces.AgreementPayment;
using MonyLoop.Domain.Interfaces.OnboardingMemberLedger;
using MonyLoop.Domain.Constants;
using MonyLoop.Domain.Interfaces.CircleRequestManagement;

namespace MonyLoop.Application.Services.AgreementPayment
{
    public class PaymentTransactionService : IPaymentTransactionService
    {
        private readonly IPaymentTransactionRepository _paymentTransactionRepository;
        private readonly IMemberLedgerRepository _memberLedgerRepository;
        private readonly ICircleRepository _circleRepository;

        private readonly IUnitOfWork _unitOfWork;

        private readonly IMapper _mapper;

        public PaymentTransactionService(
        IPaymentTransactionRepository paymentTransactionRepository,
        IMemberLedgerRepository memberLedgerRepository,
        ICircleRepository circleRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
            {
                _paymentTransactionRepository = paymentTransactionRepository;
                _memberLedgerRepository = memberLedgerRepository;
            _circleRepository = circleRepository;
            _unitOfWork = unitOfWork;
                _mapper = mapper;
            }

        // GET PAYMENTS OVERVIEW
        public async Task<PaymentsOverviewResponse> GetPaymentsByMemberLedgerAsync(
    Guid memberLedgerId,
    Guid requesterId,
    bool isAdmin)
        {
            // Check that the logged-in user is allowed
            // to access this member ledger.
            await EnsurePaymentAccessAsync(memberLedgerId,requesterId,isAdmin);

            // Load the member ledger with its assigned circle slot.
            var memberLedger =await _memberLedgerRepository.GetByIdWithSlotAsync(memberLedgerId);

            if (memberLedger is null)
            {
                throw new KeyNotFoundException( "Member ledger was not found.");
            }

            if (memberLedger.CircleSlot is null)
            {
                throw new InvalidOperationException("The member ledger is not assigned to a circle slot.");
            }

            // Load all payment transactions for this ledger.
            var transactions =await _paymentTransactionRepository.GetByMemberLedgerIdAsync(memberLedgerId);

            // Load the circle so we can obtain its contribution amount.
            var circle =await _circleRepository.GetDetailsByIdAsync(memberLedger.CircleSlot.CircleId);

            if (circle is null)
            {
                throw new InvalidOperationException("The circle related to this member ledger was not found.");
            }

            // Successful Pay-Ins are used for total paid
            // and contribution count.
            var successfulPayIns =transactions
                    .Where(x =>
                        x.TransactionType ==
                            PaymentTransactionType.PayIn &&
                        x.TransactionStatus ==
                            PaymentTransactionStatus.Successful)
                    .ToList();

            // Determine whether the member has already
            // received a successful payout.
            var hasSuccessfulPayout =transactions.Any(x =>
                    x.TransactionType ==
                        PaymentTransactionType.PayOut &&
                    x.TransactionStatus ==
                        PaymentTransactionStatus.Successful);

            var response =new PaymentsOverviewResponse
                {
                    MemberLedgerId = memberLedgerId,
                    NextContributionAmount =circle.Amount,
                    TotalPaid =successfulPayIns.Sum(x => x.Amount),
                    PaidContributionsCount =successfulPayIns.Count,
                    PayoutSlot =memberLedger.CircleSlot.SlotNumber,
                    PayoutStatus =hasSuccessfulPayout? "Paid": "Pending",
                    Transactions = _mapper.Map< List<PaymentTransactionResponse>>(transactions)
                };

            return response;
        }

        // RECORD PAY-IN
        public async Task<PaymentTransactionResponse> RecordPayInAsync(RecordPayInRequest request,
    Guid recordedByUserId, bool isAdmin)
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
            if (memberLedger.CircleSlot.Status != CircleSlotStatus.Assigned ||
             memberLedger.CircleSlot.MemberLedgerId != memberLedger.MemberLedgerId)
            {
                throw new InvalidOperationException(
                    "The member ledger does not have an active assigned circle slot.");
            }
            await EnsurePaymentAccessAsync(memberLedger.MemberLedgerId, recordedByUserId, isAdmin);
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
                RecordedByUserId = recordedByUserId,
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
        public async Task<PaymentTransactionResponse> RecordPayOutAsync(RecordPayOutRequest request,
    Guid recordedByUserId, bool isAdmin)
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
            if (memberLedger.CircleSlot.Status != CircleSlotStatus.Assigned ||
    memberLedger.CircleSlot.MemberLedgerId != memberLedger.MemberLedgerId)
            {
                throw new InvalidOperationException(
                    "The member ledger does not have an active assigned circle slot.");
            }
            await EnsurePaymentAccessAsync(memberLedger.MemberLedgerId,recordedByUserId,isAdmin);

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
                RecordedByUserId = recordedByUserId,
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
        public async Task<PaymentReceiptResponse?> GetReceiptAsync(Guid transactionId,
    Guid requesterId,bool isAdmin)
        {
            var transaction = await _paymentTransactionRepository.GetByIdAsync(transactionId);
            if (transaction is null)
                return null;
            await EnsurePaymentAccessAsync(transaction.MemberLedgerId,requesterId,isAdmin);
            return _mapper.Map<PaymentReceiptResponse>(transaction);
        }

        // HELPERS

        private async Task EnsurePaymentAccessAsync(
    Guid memberLedgerId,
    Guid requesterId,
    bool isAdmin)
        {
            var memberLedger =
                await _memberLedgerRepository
                    .GetByIdWithSlotAsync(memberLedgerId);

            if (memberLedger is null)
            {
                throw new KeyNotFoundException(
                    "Member ledger was not found.");
            }

            // Admin may access any ledger.
            if (isAdmin)
            {
                return;
            }

            // Member may access their own ledger.
            if (memberLedger.UserId == requesterId)
            {
                return;
            }

            if (memberLedger.CircleSlot is null)
            {
                throw new InvalidOperationException(
                    "The member ledger is not assigned to a circle slot.");
            }

            var circle =
                await _circleRepository.GetDetailsByIdAsync(
                    memberLedger.CircleSlot.CircleId);

            if (circle is null)
            {
                throw new InvalidOperationException(
                    "The circle related to this member ledger was not found.");
            }

            var organizerId =
                circle.CircleRequest?.CreatedByOrganizerId;

            // Organizer may access ledgers belonging to their own circle.
            if (organizerId == requesterId)
            {
                return;
            }

            throw new UnauthorizedAccessException(
                "You are not authorized to access this payment information.");
        }
        private static string GenerateReceiptNumber()
        {
            return
                $"RCPT-{DateTime.UtcNow:yyyyMMddHHmmss}-" +
                $"{Guid.NewGuid().ToString("N")[..6].ToUpper()}";
        }
    }
}