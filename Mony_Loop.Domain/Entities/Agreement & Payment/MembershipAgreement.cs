using Mony_Loop.Domain.Constants.Agreement___Payment;
using Mony_Loop.Domain.Entities.Marketplace___Applications;
using Mony_Loop.Domain.Entities.Onboarding___Member_Ledger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mony_Loop.Domain.Entities.Agreement___Payment
{
    public class MembershipAgreement
    {


        //        Attribute Type
        //Id Guid
        //MembershipApplicationId Guid(FK → MembershipApplication)
        //MemberName string
        //CircleTitle string
        //ContributionSchedule    string
        //PayoutSlot  int
        //StartDate   DateOnly
        //ExpiryDate  DateOnly
        //Status  AgreementStatus(enum)
        //CreatedAt DateTime
        //RespondedAt DateTime?

        public Guid MembershipAgreementId { get; set; }
        public Guid MembershipApplicationId { get; set; }
        public String MemberName { get; set; } = String.Empty;
        public String CircleTitle { get; set; } = String.Empty;
        public String ContributionSchedule { get; set; } = String.Empty;
        public int PayoutSlot { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly ExpiryDate { get; set; }
        public string Status { get; set; } = AgreementStatus.PendingSign;
        public DateTime CreatedAt { get; set; }
        public DateTime? RespondedAt { get; set; }

        // Navigation Properties
        public MembershipApplication? MembershipApplication { get; set; }
        public ICollection<OnboardingCase> OnboardingCases { get; set; } = new List<OnboardingCase>();
    }
}
