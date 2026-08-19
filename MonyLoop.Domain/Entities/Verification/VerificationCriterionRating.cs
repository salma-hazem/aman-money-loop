using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Domain.Entities.Verification
{
    public class VerificationCriterionRating
    {

        //        Attribute Type
        //Id Guid
        //SubmissionId Guid(FK → VerificationChecklistSubmission)
        //CriterionId Guid(FK → VerificationCriterion)
        //Rating int
        //Comments    string?

        public Guid VerificationCriterionRatingId { get; set; }
        public Guid VerificationChecklistSubmissionId { get; set; }
        public Guid VerificationCriterionId { get; set; }
        public int Rating { get; set; }
        public string? Comments { get; set; }

        // Navigation Properties
        public VerificationChecklistSubmission? VerificationChecklistSubmission { get; set; }

        public VerificationCriterion? VerificationCriterion { get; set; }

    }
}
