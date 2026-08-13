using Mony_Loop.Domain.Constants.Onboarding___Member_Ledger;
using Mony_Loop.Domain.Entities.Agreement___Payment;
using MonyLoop.Domain.Entities.UserAuth;

namespace Mony_Loop.Domain.Entities.Onboarding___Member_Ledger
{
    public class OnboardingCase
    {
        public Guid OnboardingCaseId { get; set; }
        public Guid MembershipAgreementId { get; set; }
        public Guid OrganizerId { get; set; }
        public OnboardingCaseStatus FinalStatus { get; set; } = OnboardingCaseStatus.Pending;
        public DateTime CreatedAt { get; set; }

        public MembershipAgreement? MembershipAgreement { get; set; }
        public ApplicationUser? Organizer { get; set; }
        public ICollection<Document> Documents { get; set; } = new List<Document>();
        public MemberLedger? MemberLedger { get; set; }
    }
}
