using Mony_Loop.Domain.Constants.Verification;
using Mony_Loop.Domain.Entities.Marketplace___Applications;

namespace Mony_Loop.Domain.Entities.Verification
{
    public class VerificationSchedule
    {
        public Guid VerificationScheduleId { get; set; }
        public Guid ApplicationId { get; set; }
        public Guid VerificationRoundId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public string? LocationLink { get; set; }
        public string? VideoLink { get; set; }
        public string Status { get; set; } = ScheduleStatus.Pending;

        public MembershipApplication? MembershipApplication { get; set; }
        public VerificationRound? VerificationRound { get; set; }
        public ICollection<VerificationChecklistSubmission> VerificationChecklistSubmissions { get; set; } = new List<VerificationChecklistSubmission>();
    }
}
