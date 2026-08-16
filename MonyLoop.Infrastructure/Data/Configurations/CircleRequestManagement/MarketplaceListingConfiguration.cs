using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MonyLoop.Domain.Constants;
using MonyLoop.Domain.Entities.CircleRequestManagement;

namespace MonyLoop.Infrastructure.Data.Configurations.CircleRequestManagement
{
    public class MarketplaceListingConfiguration : IEntityTypeConfiguration<MarketplaceListing>
    {
        public void Configure(EntityTypeBuilder<MarketplaceListing> builder)
        {
            builder.ToTable("MarketplaceListings");

            builder.HasKey(x => x.ListingId);

            builder.Property(x => x.CircleId)
                .IsRequired();

            builder.Property(x => x.ListingStatus)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50)
                .HasDefaultValue(MarketplaceListingStatus.Active);

            builder.HasIndex(x => x.CircleId)
                .IsUnique();

            builder.HasIndex(x => x.ListingStatus);

            // MVP rule: one circle has one marketplace listing.
            builder.HasOne(x => x.Circle)
                .WithOne(x => x.MarketplaceListing)
                .HasForeignKey<MarketplaceListing>(x => x.CircleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
