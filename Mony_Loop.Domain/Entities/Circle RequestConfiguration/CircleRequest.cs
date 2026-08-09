using Mony_Loop.Domain.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mony_Loop.Domain.Entities.Circle_Request___Configuration
{
    public class CircleRequest
    {
        //        Attribute Type
        //Id Guid
        //ExistingCircleId Guid? (FK → Circle, nullable — بس للـ Replacement)
        //CreatedByOrganizerId    Guid 🆕 (FK → User)
        //ReviewedByAdminId Guid? (FK → User)
        //CircleTitle string
        //CircleType  CircleType(enum)
        //ContributionAmount decimal
        //Duration    int
        //NumberOfSlots   int
        //ShortJustification  string?
        //RequestStatus   CircleRequestStatus(enum)
        //VacantSlotNumber int?
        //CreatedAt DateTime
        //SubmittedAt DateTime?
        //ReviewedAt DateTime?
        //DecisionReason string?
        public Guid RequestId { get; set; }
        public Guid? ExistingCircleId { get; set; }
        public Guid CreatedByOrganizerId { get; set; }
        public Guid? ReviewedByAdminId { get; set; }
        public string Circletitle { get; set; } = String.Empty;
        public string Circletype { get; set; } = CircleTypes.NewCircle;
        public decimal ContributionAmount { get; set; }
        public int Duration { get; set; }
        public int NumberOfSlots { get; set; }
        public string? ShortJustification { get; set; }
        public String RequestStatus { get; set; } = CircleRequestStatus.Draft;
        public int? VacantSlotNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? DecisionReason { get; set; }


        // navigation properties
        public Circle? ExistingCircle { get; set; }

        //public User? CreatedByOrganizer { get; set; }

        //public User? ReviewedByAdmin { get; set; }
    }



}
