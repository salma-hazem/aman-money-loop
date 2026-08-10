
using Mony_Loop.Domain.Constants;
using Mony_Loop.Domain.Entities.Agreement___Payment;
using Mony_Loop.Domain.Entities.Circle_Request___Configuration;
using Mony_Loop.Domain.Entities.Verification;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mony_Loop.Domain.Entities.Marketplace___Applications
{
    public class MembershipApplication
    {

        //        MembershipApplication
        //Attribute   Type
        //Id  Guid
        //UserId  Guid? (FK → User, nullable للـ Guest)
        //CircleId Guid(FK → Circle)
        //Name string
        //Email   string
        //Phone   string
        //NationalId  string
        //Stage   MembershipApplicationStage(enum)
        //CreatedAt DateTime
        //UpdatedAt DateTime?

        public Guid MembershipApplicationId { get; set; }
        public Guid UserId { get; set; }
        public Guid CircleId { get; set; }
        public String Name { get; set; } = String.Empty;
        public String Email { get; set; } = String.Empty;
        public String Phone { get; set; } = String.Empty;
        public String NationalId { get; set; } = String.Empty;
        public String Stage { get; set; } = MembershipApplicationStage.Submitted;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }


        // Navigation Properties
        public Circle? Circle { get; set; }

        // public User? User { get; set; }

        public ICollection<VerificationSchedule> VerificationSchedules { get; set; } = new List<VerificationSchedule>();

        public ICollection<MembershipAgreement> MembershipAgreements { get; set; } = new List<MembershipAgreement>();

    }
}
