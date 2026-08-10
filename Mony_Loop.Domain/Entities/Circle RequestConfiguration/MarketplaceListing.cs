using Mony_Loop.Domain.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mony_Loop.Domain.Entities.Circle_Request___Configuration
{
    public class MarketplaceListing
    {
        //        Attribute Type
        //Id Guid
        //CircleId Guid(FK → Circle)
        //ListingStatus ListingStatus(enum)

        public Guid ListingId { get; set; }
        public Guid CircleId { get; set; }
        public String ListingStatus { get; set; } = MarketplaceListingStatus.Active;

        // navigation property
        public Circle? Circle { get; set; }

    }
}
