namespace MonyLoop.Application.DTOs.AgreementPayment.PaymentTransaction
{
    public class PaymentTransactionResponse
    {
        public Guid PaymentTransactionId { get; set; }

        public string TransactionType { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        public string TransactionStatus { get; set; } = string.Empty;

        public DateTime TransactionDate { get; set; }

        public string? ReceiptNumber { get; set; }
    }
}