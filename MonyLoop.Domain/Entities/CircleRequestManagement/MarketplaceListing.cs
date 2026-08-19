using MonyLoop.Domain.Constants;
using MonyLoop.Domain.Entities.Marketplace___Applications;

namespace MonyLoop.Domain.Entities.CircleRequestManagement
{
    public class MarketplaceListing
    {
        public Guid ListingId { get; set; }
        public Guid CircleId { get; set; }
        public MarketplaceListingStatus ListingStatus { get; set; } = MarketplaceListingStatus.Active;

        public Circle? Circle { get; set; }
        public ICollection<MembershipApplication> MembershipApplications { get; set; } = new List<MembershipApplication>();
    }
}
