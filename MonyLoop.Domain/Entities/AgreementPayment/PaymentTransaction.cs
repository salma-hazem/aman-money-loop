using MonyLoop.Domain.Constants.Agreement___Payment;
using MonyLoop.Domain.Entities.CircleRequestManagement;
using MonyLoop.Domain.Entities.Onboarding___Member_Ledger;

namespace MonyLoop.Domain.Entities.Agreement___Payment
{
    public class PaymentTransaction
    {
        public Guid PaymentTransactionId { get; set; }
        public Guid MemberLedgerId { get; set; }
        public Guid CircleId { get; set; }
        public Guid RecordedByUserId { get; set; }
        public PaymentTransactionType TransactionType { get; set; } = PaymentTransactionType.PayIn;
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.EWallet;
        public PaymentTransactionStatus TransactionStatus { get; set; } = PaymentTransactionStatus.Pending;
        public decimal Amount { get; set; }
        public string? TransactionReference { get; set; }
        public string? ReceiptNumber { get; set; }
        public string? ReceiptFilePath { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime CreatedAt { get; set; }

        public Circle? Circle { get; set; }
        public MemberLedger? MemberLedger { get; set; }
        // public User? RecordedByUser { get; set; }
    }
}
