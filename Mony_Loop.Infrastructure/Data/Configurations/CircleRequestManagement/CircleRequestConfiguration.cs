using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mony_Loop.Domain.Constants;
using Mony_Loop.Domain.Entities.CircleRequestManagement;
using MonyLoop.Domain.Entities.UserAuth;

namespace Mony_Loop.Infrastructure.Data.Configurations.CircleRequestManagement
{
    public class CircleRequestConfiguration : IEntityTypeConfiguration<CircleRequest>
    {
        public void Configure(EntityTypeBuilder<CircleRequest> builder)
        {
            builder.ToTable("CircleRequests");

            builder.HasKey(x => x.RequestId);

            builder.Property(x => x.ExistingCircleId)
                .IsRequired(false);

            builder.Property(x => x.CreatedByOrganizerId)
                .IsRequired();

            builder.Property(x => x.ReviewedByAdminId)
                .IsRequired(false);

            builder.Property(x => x.CircleTitle)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(x => x.CircleType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50)
                .HasDefaultValue(CircleType.NewCircle);

            builder.Property(x => x.ContributionAmount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(x => x.Duration)
                .IsRequired();

            builder.Property(x => x.NumberOfSlots)
                .IsRequired();

            builder.Property(x => x.ShortJustification)
                .IsRequired(false)
                .HasMaxLength(500);

            builder.Property(x => x.RequestStatus)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50)
                .HasDefaultValue(CircleRequestStatus.Draft);

            builder.Property(x => x.VacantSlotNumber)
                .IsRequired(false);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.SubmittedAt)
                .IsRequired(false);

            builder.Property(x => x.ReviewedAt)
                .IsRequired(false);

            builder.Property(x => x.DecisionReason)
                .IsRequired(false)
                .HasMaxLength(1000);

            builder.HasIndex(x => x.CreatedByOrganizerId);
            builder.HasIndex(x => x.ReviewedByAdminId);
            builder.HasIndex(x => x.RequestStatus);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.CreatedByOrganizerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.ReviewedByAdminId)
                .OnDelete(DeleteBehavior.Restrict);

            // ExistingCircleId is used only for replacement requests.
            builder.HasOne(x => x.ExistingCircle)
                .WithMany(x => x.ReplacementRequests)
                .HasForeignKey(x => x.ExistingCircleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
