using MonyLoop.Domain.Constants;
using MonyLoop.Domain.Entities.Agreement___Payment;
using MonyLoop.Domain.Entities.Verification;

namespace MonyLoop.Domain.Entities.CircleRequestManagement
{
    public class Circle
    {
        public Guid CircleId { get; set; }
        public Guid RequestId { get; set; }
        public int ApprovedSlots { get; set; }
        public int FilledCount { get; set; }
        public decimal Amount { get; set; }
        public int Duration { get; set; }
        public CircleStatus Status { get; set; } = CircleStatus.Open;

        public CircleRequest? CircleRequest { get; set; }
        public MarketplaceListing? MarketplaceListing { get; set; }
        public ICollection<CircleRequest> ReplacementRequests { get; set; } = new List<CircleRequest>();
        public ICollection<CircleSlot> CircleSlots { get; set; } = new List<CircleSlot>();
        public ICollection<VerificationRound> VerificationRounds { get; set; } = new List<VerificationRound>();
        public ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
    }
}
