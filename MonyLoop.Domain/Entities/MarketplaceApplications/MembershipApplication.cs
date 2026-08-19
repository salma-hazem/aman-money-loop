using MonyLoop.Domain.Constants;
using MonyLoop.Domain.Entities.Agreement___Payment;
using MonyLoop.Domain.Entities.CircleRequestManagement;
using MonyLoop.Domain.Entities.Verification;

namespace MonyLoop.Domain.Entities.Marketplace___Applications
{
    public class MembershipApplication
    {
        public Guid MembershipApplicationId { get; set; }
        public Guid? UserId { get; set; }
        public Guid ListingId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string NationalId { get; set; } = string.Empty;
        public MembershipApplicationStage Stage { get; set; } = MembershipApplicationStage.Submitted;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // public User? User { get; set; }
        public MarketplaceListing? MarketplaceListing { get; set; }
        public ICollection<VerificationSchedule> VerificationSchedules { get; set; } = new List<VerificationSchedule>();
        public MembershipAgreement? MembershipAgreement { get; set; }
    }
}
