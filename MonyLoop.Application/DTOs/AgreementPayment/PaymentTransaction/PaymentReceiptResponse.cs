namespace MonyLoop.Application.DTOs.AgreementPayment.PaymentTransaction
{
    public class PaymentReceiptResponse
    {
        public Guid PaymentTransactionId { get; set; }

        public string? ReceiptNumber { get; set; }

        public decimal Amount { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        public string? TransactionReference { get; set; }

        public DateTime TransactionDate { get; set; }
    }
}