using Mony_Loop.Domain.Constants.Verification;
using Mony_Loop.Domain.Entities.Circle_Request___Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Mony_Loop.Domain.Entities.Verification
{
    public class VerificationRound
    {

        //        VerificationRound
        //Attribute   Type
        //Id  Guid
        //CircleId    Guid(FK → Circle)
        //ReviewedByUserId Guid(FK → User)
        //RoundName string
        //Format  VerificationFormat(enum)

        public Guid VerificationRoundId { get; set; }
        public Guid CircleId { get; set; }
        public Guid ReviewedByUserId { get; set; }
        public string RoundName { get; set; } = string.Empty;
        public string Format { get; set; } = VerificationFormats.Online;

        // Navigation Properties
        public Circle? Circle { get; set; }

        // public User? ReviewedByUser { get; set; }
        public ICollection<VerificationSchedule> VerificationSchedules { get; set; } = new List<VerificationSchedule>();

        public ICollection<VerificationCriterion> VerificationCriteria { get; set; } = new List<VerificationCriterion>();


    }
}
