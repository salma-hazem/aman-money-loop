using MonyLoop.Domain.Entities.Agreement___Payment;
using MonyLoop.Domain.Entities.CircleRequestManagement;
using MonyLoop.Domain.Entities.UserAuth;

namespace MonyLoop.Domain.Entities.Onboarding___Member_Ledger
{
    public class MemberLedger
    {
        public Guid MemberLedgerId { get; set; }
        public Guid UserId { get; set; }
        public Guid OnboardingCaseId { get; set; }
        public Guid ActivatedByAdminId { get; set; }
        public Guid? CircleSlotId { get; set; }
        public DateTime ActivatedAt { get; set; }

        public OnboardingCase? OnboardingCase { get; set; }
        public ApplicationUser? User { get; set; }
        public ApplicationUser? ActivatedByAdmin { get; set; }
        public CircleSlot? CircleSlot { get; set; }
        public ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
    }
}
