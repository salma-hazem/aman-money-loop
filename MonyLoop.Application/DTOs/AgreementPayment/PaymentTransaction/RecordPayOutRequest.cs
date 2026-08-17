using MonyLoop.Domain.Constants.Agreement___Payment;

namespace MonyLoop.Application.DTOs.AgreementPayment.PaymentTransaction
{
    public class RecordPayOutRequest
    {
        public Guid MemberLedgerId { get; set; }

        public decimal Amount { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public string? TransactionReference { get; set; }
    }
}