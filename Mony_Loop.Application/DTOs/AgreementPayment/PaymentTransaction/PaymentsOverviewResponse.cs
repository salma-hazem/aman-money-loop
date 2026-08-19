namespace Mony_Loop.Application.DTOs.AgreementPayment.PaymentTransaction
{
    public class PaymentsOverviewResponse
    {
        public Guid MemberLedgerId { get; set; }

        // Next Contribution
        public decimal? NextContributionAmount { get; set; }
        public DateOnly? NextContributionDueDate { get; set; }

        // Total Paid
        public decimal TotalPaid { get; set; }
        public int PaidContributionsCount { get; set; }

        // Payout
        public int? PayoutSlot { get; set; }
        public string? PayoutStatus { get; set; }

        // Transactions
        public List<PaymentTransactionResponse> Transactions { get; set; }
            = new();
    }
}