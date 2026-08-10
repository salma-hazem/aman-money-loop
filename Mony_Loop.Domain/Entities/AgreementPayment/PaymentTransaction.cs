using Mony_Loop.Domain.Constants;
using Mony_Loop.Domain.Constants.Agreement___Payment;
using Mony_Loop.Domain.Entities.Circle_Request___Configuration;
using Mony_Loop.Domain.Entities.Onboarding___Member_Ledger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mony_Loop.Domain.Entities.Agreement___Payment
{
    public class PaymentTransaction
    {
        //        PaymentTransaction
        //Attribute   Type
        //Id  Guid
        //MemberLedgerId  Guid(FK → MemberLedger) ✏️ (مش AgreementID زي الأصل)
        //CircleId Guid(FK → Circle)
        //RecordedByUserId    Guid(FK → User)
        //TransactionType TransactionType(enum)
        //Amount decimal
        //PaymentMethod   PaymentMethod(enum)
        //TransactionReference string
        //TransactionStatus   TransactionStatus(enum)
        //ReceiptNumber string
        //ReceiptFilePath string
        //TransactionDate DateTime
        //CreatedAt DateTime

        public Guid PaymentTransactionId { get; set; }
        public Guid MemberLedgerId { get; set; }
        public Guid CircleId { get; set; }
        public Guid RecordedByUserId { get; set; }
        public string TransactionType { get; set; } = PaymentTransactionTypes.Deposit;
        public string PaymentMethod { get; set; } = PaymentMethods.InstaPay;
        public string TransactionStatus { get; set; } = PaymentTransactionStatus.Pending;
        public decimal Amount { get; set; }
        public string TransactionReference { get; set; } = string.Empty;
        public string ReceiptNumber { get; set; } = string.Empty;
        public string ReceiptFilePath { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation Properties

        public Circle? Circle { get; set; }

        public MemberLedger? MemberLedger { get; set; }

        // public User? RecordedByUser { get; set; }

    }
}
