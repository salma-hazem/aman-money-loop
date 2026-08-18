namespace MonyLoop.Application.DTOs.AgreementPayment.PaymentTransaction
{
    public class PaymentsOverviewResponse
    {
        public Guid MemberLedgerId { get; set; }

        public decimal? NextContributionAmount { get; set; }

        public DateOnly? NextContributionDueDate { get; set; }

        public decimal TotalPaid { get; set; }

        public int PaidContributionsCount { get; set; }

        public int? PayoutSlot { get; set; }

        public string? PayoutStatus { get; set; }

        public List<PaymentTransactionResponse> Transactions { get; set; }
            = new();
    }
}