using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Mony_Loop.Domain.Constants.Verification;
using Mony_Loop.Domain.Entities.Marketplace___Applications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Mony_Loop.Domain.Entities.Verification
{
    public class VerificationSchedule
    {
        //        VerificationSchedule
        //Attribute   Type
        //Id  Guid
        //ApplicationId   Guid(FK → MembershipApplication)
        //RoundId Guid(FK → VerificationRound)
        //Date DateOnly
        //Time TimeOnly
        //LocationLink string?
        //VideoLink   string?
        //Status  ScheduleStatus(enum)
        public Guid VerificationScheduleId { get; set; }
        public Guid ApplicationId { get; set; }
        public Guid VerificationRoundId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public string? LocationLink { get; set; }
        public string? VideoLink { get; set; }
        public string Status { get; set; } = ScheduleStatus.Pending;

        // Navigation Properties
        public MembershipApplication? MembershipApplication { get; set; }

        public VerificationRound? VerificationRound { get; set; }
        public ICollection<VerificationChecklistSubmission> VerificationChecklistSubmissions { get; set; } = new List<VerificationChecklistSubmission>();


    }
}
