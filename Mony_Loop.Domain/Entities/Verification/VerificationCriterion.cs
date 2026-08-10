using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mony_Loop.Domain.Entities.Verification
{
    public class VerificationCriterion
    {

        //        Attribute Type
        //Id Guid
        //RoundId Guid(FK → VerificationRound)
        //CriterionName string
        //Weight  decimal
        //DisplayOrder    int
        //IsActive    bool

        public Guid VerificationCriterionId { get; set; }
        public Guid VerificationRoundId { get; set; }
        public string CriterionName { get; set; } = String.Empty;
        public decimal Weight { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        // Navigation Properties
        public VerificationRound? VerificationRound { get; set; }
        public ICollection<VerificationCriterionRating> VerificationCriterionRatings { get; set; } = new List<VerificationCriterionRating>();


    }
}
