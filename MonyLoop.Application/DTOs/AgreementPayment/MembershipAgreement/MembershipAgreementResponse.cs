using MonyLoop.Domain.Constants.Agreement___Payment;

namespace MonyLoop.Application.DTOs.AgreementPayment.MembershipAgreement
{
    public class MembershipAgreementResponse
    {
        public Guid MembershipAgreementId { get; set; }

        public Guid MembershipApplicationId { get; set; }

        public string MemberName { get; set; } = string.Empty;

        public string CircleTitle { get; set; } = string.Empty;

        public string ContributionSchedule { get; set; } = string.Empty;

        public int PayoutSlot { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly ExpiryDate { get; set; }

        public AgreementStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? RespondedAt { get; set; }
    }
}