using Mony_Loop.Domain.Constants.Agreement___Payment;
using Mony_Loop.Domain.Entities.Circle_Request___Configuration;
using Mony_Loop.Domain.Entities.Onboarding___Member_Ledger;

namespace Mony_Loop.Domain.Entities.Agreement___Payment
{
    public class PaymentTransaction
    {
        public Guid PaymentTransactionId { get; set; }
        public Guid MemberLedgerId { get; set; }
        public Guid CircleId { get; set; }
        public Guid RecordedByUserId { get; set; }
        public string TransactionType { get; set; } = PaymentTransactionTypes.PayIn;
        public string PaymentMethod { get; set; } = PaymentMethods.EWallet;
        public string TransactionStatus { get; set; } = PaymentTransactionStatus.Pending;
        public decimal Amount { get; set; }
        public string TransactionReference { get; set; } = string.Empty;
        public string ReceiptNumber { get; set; } = string.Empty;
        public string ReceiptFilePath { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public DateTime CreatedAt { get; set; }

        public Circle? Circle { get; set; }
        public MemberLedger? MemberLedger { get; set; }
        // public User? RecordedByUser { get; set; }
    }
}
