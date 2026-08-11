using Mony_Loop.Domain.Constants;
using Mony_Loop.Domain.Entities.Onboarding___Member_Ledger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mony_Loop.Domain.Entities.CircleRequestManagement
{
    public class CircleSlot
    {
        //        Attribute Type
        //Id Guid
        //CircleId Guid(FK ? Circle)
        //MemberLedgerId Guid? (FK ? MemberLedger, nullable)
        //SlotNumber int
        //Status  SlotStatus(enum)
        //VacatedAt DateTime?
        //AssignedAt DateTime?

        public Guid CircleSlotId { get; set; }
        public Guid CircleId { get; set; }
        public Guid? MemberLedgerId { get; set; }
        public int SlotNumber { get; set; }
        public CircleSlotStatus Status { get; set; } = CircleSlotStatus.Vacant;
        public DateTime? VacatedAt { get; set; }
        public DateTime? AssignedAt { get; set; }

        // navigation property
        public Circle? Circle { get; set; }
        public MemberLedger? MemberLedger { get; set; }
    }
}
