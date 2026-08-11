using Mony_Loop.Domain.Constants;

namespace Mony_Loop.Domain.Entities.Circle_Request___Configuration
{
    public class CircleRequest
    {
        public Guid RequestId { get; set; }
        public Guid? ExistingCircleId { get; set; }
        public Guid CreatedByOrganizerId { get; set; }
        public Guid? ReviewedByAdminId { get; set; }
        public string CircleTitle { get; set; } = string.Empty;
        public CircleType CircleType { get; set; } = CircleType.NewCircle;
        public decimal ContributionAmount { get; set; }
        public int Duration { get; set; }
        public int NumberOfSlots { get; set; }
        public string? ShortJustification { get; set; }
        public CircleRequestStatus RequestStatus { get; set; } = CircleRequestStatus.Draft;
        public int? VacantSlotNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? DecisionReason { get; set; }

        public Circle? ExistingCircle { get; set; }
        // public User? CreatedByOrganizer { get; set; }
        // public User? ReviewedByAdmin { get; set; }
    }
}
