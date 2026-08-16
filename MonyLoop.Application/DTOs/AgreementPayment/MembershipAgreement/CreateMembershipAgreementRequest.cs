namespace MonyLoop.Application.DTOs.AgreementPayment.MembershipAgreement
{
    public class CreateMembershipAgreementRequest
    {
        public Guid MembershipApplicationId { get; set; }

        public string ContributionSchedule { get; set; } = string.Empty;

        public int PayoutSlot { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly ExpiryDate { get; set; }
    }
}
