using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mony_Loop.Domain.Entities.Verification
{
    public class VerificationChecklistSubmission
    {

        //        Attribute Type
        //Id Guid
        //VerificationScheduleId Guid(FK → VerificationSchedule)
        //SubmittedByUserId Guid(FK → User)
        //CompositeScore decimal
        //OverallComments string?
        //SubmittedAt DateTime

        public Guid VerificationChecklistSubmissionId { get; set; }
        public Guid VerificationScheduleId { get; set; }
        public Guid SubmittedByUserId { get; set; }
        public decimal CompositeScore { get; set; }
        public string? OverallComments { get; set; }
        public DateTime SubmittedAt { get; set; }

        // Navigation Properties
        public VerificationSchedule? VerificationSchedule { get; set; }

        // public User? SubmittedByUser { get; set; }

        public ICollection<VerificationCriterionRating> VerificationCriterionRatings { get; set; } = new List<VerificationCriterionRating>();


    }
}
