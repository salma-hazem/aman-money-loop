using MonyLoop.Domain.Constants.Onboarding___Member_Ledger;
using MonyLoop.Domain.Entities.Agreement___Payment;
using MonyLoop.Domain.Entities.UserAuth;

namespace MonyLoop.Domain.Entities.Onboarding___Member_Ledger
{
    public class OnboardingCase
    {
        public Guid OnboardingCaseId { get; set; }
        public Guid MembershipAgreementId { get; set; }
        public Guid OrganizerId { get; set; }
        public Guid UserId { get; set; }
        public OnboardingCaseStatus FinalStatus { get; set; } = OnboardingCaseStatus.Pending;
        public DateTime CreatedAt { get; set; }

        public MembershipAgreement? MembershipAgreement { get; set; }
        public ApplicationUser? Organizer { get; set; }
        public ICollection<Document> Documents { get; set; } = new List<Document>();
        public MemberLedger? MemberLedger { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
