using Mony_Loop.Domain.Constants;

namespace Mony_Loop.Application.DTOs
{
    // Screen 9 - Circle Application Form (submit)
    public class CreateMembershipApplicationDto
    {
        public Guid ListingId { get; set; }
        public Guid? UserId { get; set; } // null when a guest applies
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string NationalId { get; set; } = string.Empty;
    }

    // Screen 17 - Applicant Pipeline (one card per applicant, grouped by Stage)
    public class MembershipApplicationSummaryDto
    {
        public Guid MembershipApplicationId { get; set; }
        public string Name { get; set; } = string.Empty;
        public MembershipApplicationStage Stage { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // Screen 18 - Applicant Details
    public class MembershipApplicationDetailDto
    {
        public Guid MembershipApplicationId { get; set; }
        public Guid ListingId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string NationalId { get; set; } = string.Empty;
        public MembershipApplicationStage Stage { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}