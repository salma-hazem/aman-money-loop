using Mony_Loop.Domain.Entities.Agreement___Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mony_Loop.Domain.Entities.Onboarding___Member_Ledger
{
    public class MemberLedger
    {

        //MemberLedger
        //AttributeType
        //Id Guid
        //UserId Guid (FK → User)
        //CaseId Guid (FK → OnboardingCase)
        //ActivatedByAdminId Guid (FK → User)
        //ActivatedAt DateTime

        public Guid MemberLedgerId { get; set; }
        public Guid UserId { get; set; }
        public Guid OnboardingCaseId { get; set; }
        public Guid ActivatedByAdminId { get; set; }
        public DateTime ActivatedAt { get; set; }

        // Navigation Properties
        public OnboardingCase? OnboardingCase { get; set; }

        public ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();

        // public User? User { get; set; }

        // public User? ActivatedByAdmin { get; set; }
    }
}
