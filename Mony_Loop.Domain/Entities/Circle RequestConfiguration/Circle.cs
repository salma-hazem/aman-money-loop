using Mony_Loop.Domain.Constants;
using Mony_Loop.Domain.Entities.Agreement___Payment;
using Mony_Loop.Domain.Entities.Verification;

namespace Mony_Loop.Domain.Entities.Circle_Request___Configuration
{
    public class Circle
    {
        public Guid CircleId { get; set; }
        public Guid RequestId { get; set; }
        public int ApprovedSlots { get; set; }
        public int FilledCount { get; set; }
        public decimal Amount { get; set; }
        public int Duration { get; set; }
        public string Status { get; set; } = CircleStatus.Open;

        public CircleRequest? CircleRequest { get; set; }
        public MarketplaceListing? MarketplaceListing { get; set; }
        public ICollection<CircleRequest> ReplacementRequests { get; set; } = new List<CircleRequest>();
        public ICollection<CircleSlot> CircleSlots { get; set; } = new List<CircleSlot>();
        public ICollection<VerificationRound> VerificationRounds { get; set; } = new List<VerificationRound>();
        public ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
    }
}
