using MonyLoop.Domain.Constants.Agreement___Payment;
using MonyLoop.Domain.Entities.Marketplace___Applications;
using MonyLoop.Domain.Entities.Onboarding___Member_Ledger;

namespace MonyLoop.Domain.Entities.Agreement___Payment
{
    public class MembershipAgreement
    {
        public Guid MembershipAgreementId { get; set; }
        public Guid MembershipApplicationId { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public string CircleTitle { get; set; } = string.Empty;
        public string ContributionSchedule { get; set; } = string.Empty;
        public int PayoutSlot { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly ExpiryDate { get; set; }
        public AgreementStatus Status { get; set; } = AgreementStatus.Pending;

        public string ResponseTokenHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? RespondedAt { get; set; }

        public MembershipApplication? MembershipApplication { get; set; }
        public OnboardingCase? OnboardingCase { get; set; }
    }
}
