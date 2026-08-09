using Mony_Loop.Domain.Constants.Onboarding___Member_Ledger;
using Mony_Loop.Domain.Entities.Agreement___Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mony_Loop.Domain.Entities.Onboarding___Member_Ledger
{
    public class OnboardingCase
    {

        //        OnboardingCase
        //Attribute   Type
        //Id  Guid
        //AgreementId Guid(FK → MembershipAgreement)
        //OrganizerId Guid(FK → User)
        //FinalStatus OnboardingCaseStatus(enum)
        //CreatedAt DateTime

        public Guid OnboardingCaseId { get; set; }
        public Guid MembershipAgreementId { get; set; }
        public Guid OrganizerId { get; set; }
        public string FinalStatus { get; set; } = OnboardingCaseStatus.Pending;
        public DateTime CreatedAt { get; set; }

        // Navigation Properties
        public MembershipAgreement? MembershipAgreement { get; set; }

        // public User? Organizer { get; set; }

        public ICollection<Document> Documents { get; set; } = new List<Document>();
        public ICollection<MemberLedger> MemberLedgers { get; set; } = new List<MemberLedger>();



    }
}
