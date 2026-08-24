using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mony_Loop.Domain.Constants;
using Mony_Loop.Domain.Entities.CircleRequestManagement;

namespace Mony_Loop.Infrastructure.Data.Configurations.CircleRequestManagement
{
    public class CircleConfiguration : IEntityTypeConfiguration<Circle>
    {
        public void Configure(EntityTypeBuilder<Circle> builder)
        {
            builder.ToTable("Circles");

            builder.HasKey(x => x.CircleId);

            builder.Property(x => x.RequestId)
                .IsRequired();

            builder.Property(x => x.ApprovedSlots)
                .IsRequired();

            builder.Property(x => x.FilledCount)
                .IsRequired();

            builder.Property(x => x.Amount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(x => x.Duration)
                .IsRequired();

            builder.Property(x => x.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50)
                .HasDefaultValue(CircleStatus.Open);

            builder.HasIndex(x => x.RequestId)
                .IsUnique();

            builder.HasIndex(x => x.Status);

            // Approved circle created from one circle request.
            builder.HasOne(x => x.CircleRequest)
                .WithOne()
                .HasForeignKey<Circle>(x => x.RequestId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
