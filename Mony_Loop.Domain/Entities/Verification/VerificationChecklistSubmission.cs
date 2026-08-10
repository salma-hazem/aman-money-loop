namespace Mony_Loop.Domain.Entities.Verification
{
    public class VerificationChecklistSubmission
    {
        public Guid VerificationChecklistSubmissionId { get; set; }
        public Guid VerificationScheduleId { get; set; }
        public Guid SubmittedByUserId { get; set; }
        public decimal CompositeScore { get; set; }
        public string? OverallComments { get; set; }
        public DateTime SubmittedAt { get; set; }

        public VerificationSchedule? VerificationSchedule { get; set; }
        // public User? SubmittedByUser { get; set; }
        public ICollection<VerificationCriterionRating> VerificationCriterionRatings { get; set; } = new List<VerificationCriterionRating>();
    }
}
