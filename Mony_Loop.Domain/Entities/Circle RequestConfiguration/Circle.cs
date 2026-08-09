using Mony_Loop.Domain.Constants;
using Mony_Loop.Domain.Entities.Agreement___Payment;
using Mony_Loop.Domain.Entities.Marketplace___Applications;
using Mony_Loop.Domain.Entities.Verification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mony_Loop.Domain.Entities.Circle_Request___Configuration
{
    public class Circle
    //    {
    //        Circle
    //Attribute   Type
    //Id  Guid
    //RequestId   Guid(FK → CircleRequest)
    //ApprovedSlots int
    //FilledCount int
    //Amount  decimal
    //Duration    int
    //Status  CircleStatus(enum)

    {
        public Guid CircleId { get; set; }
        public Guid RequestId { get; set; }
        public int ApprovedSlots { get; set; }
        public int FilledCount { get; set; }
        public decimal Amount { get; set; }
        public int Duration { get; set; }
        public String Status { get; set; } = CircleStatus.Open;

        // Navigation property to 

        public CircleRequest? CircleRequest { get; set; }

        public ICollection<CircleRequest> ReplacementRequests { get; set; } = new List<CircleRequest>();
        //ReplacementRequests كـ Collection عشان لو الجمعية دي اتفتح فيها مكان فاضي (Vacant Slot)،
        //يترمى عليها طلبات إحلال (Replacement Requests) من كلاس الـ CircleRequest اللي لسه مخلصينه سوا.
        public ICollection<CircleSlot> CircleSlots { get; set; } = new List<CircleSlot>();
        public ICollection<MarketplaceListing> MarketplaceListings { get; set; } = new List<MarketplaceListing>();
        public ICollection<MembershipApplication> MembershipApplications { get; set; } = new List<MembershipApplication>();

        public ICollection<VerificationRound> VerificationRounds { get; set; } = new List<VerificationRound>();

        public ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();




    }
}
