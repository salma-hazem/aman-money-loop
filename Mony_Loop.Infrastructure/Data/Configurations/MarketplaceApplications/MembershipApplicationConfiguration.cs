using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mony_Loop.Domain.Constants;
using Mony_Loop.Domain.Entities.Marketplace___Applications;

namespace Mony_Loop.Infrastructure.Data.Configurations.MarketplaceApplications
{
    public class MembershipApplicationConfiguration
        : IEntityTypeConfiguration<MembershipApplication>
    {
        public void Configure(EntityTypeBuilder<MembershipApplication> builder)
        {
            builder.ToTable("MembershipApplications");

            builder.HasKey(x => x.MembershipApplicationId);

            builder.Property(x => x.UserId)
                .IsRequired(false);

            builder.Property(x => x.ListingId)
                .IsRequired();

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.Phone)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.NationalId)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.Stage)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50)
                .HasDefaultValue(MembershipApplicationStage.Submitted);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .IsRequired(false);

            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.ListingId);
            builder.HasIndex(x => x.Stage);

            // Guests (UserId == null) can apply; the target Circle is reached
            // via MembershipApplication -> MarketplaceListing -> Circle.
            builder.HasOne(x => x.MarketplaceListing)
                .WithMany(x => x.MembershipApplications)
                .HasForeignKey(x => x.ListingId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}