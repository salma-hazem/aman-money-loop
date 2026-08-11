using Mony_Loop.Domain.Constants.Verification;
using Mony_Loop.Domain.Entities.Circle_Request___Configuration;

namespace Mony_Loop.Domain.Entities.Verification
{
    public class VerificationRound
    {
        public Guid VerificationRoundId { get; set; }
        public Guid CircleId { get; set; }
        public Guid ReviewedByUserId { get; set; }
        public string RoundName { get; set; } = string.Empty;
        public VerificationFormat Format { get; set; } = VerificationFormat.Video;

        public Circle? Circle { get; set; }
        // public User? ReviewedByUser { get; set; }
        public ICollection<VerificationSchedule> VerificationSchedules { get; set; } = new List<VerificationSchedule>();
        public ICollection<VerificationCriterion> VerificationCriteria { get; set; } = new List<VerificationCriterion>();
    }
}
