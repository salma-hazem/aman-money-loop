using Mony_Loop.Domain.Constants;
using Mony_Loop.Domain.Entities.Agreement___Payment;
using Mony_Loop.Domain.Entities.Circle_Request___Configuration;
using Mony_Loop.Domain.Entities.Verification;

namespace Mony_Loop.Domain.Entities.Marketplace___Applications
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
