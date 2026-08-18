using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MonyLoop.Domain.Constants;
using MonyLoop.Domain.Entities.CircleRequestManagement;

namespace MonyLoop.Infrastructure.Data.Configurations.CircleRequestManagement
{
    public class CircleSlotConfiguration : IEntityTypeConfiguration<CircleSlot>
    {
        public void Configure(EntityTypeBuilder<CircleSlot> builder)
        {
            builder.ToTable("CircleSlots");

            builder.HasKey(x => x.CircleSlotId);

            builder.Property(x => x.CircleId)
                .IsRequired();

            builder.Property(x => x.MemberLedgerId)
                .IsRequired(false);

            builder.Property(x => x.SlotNumber)
                .IsRequired();

            builder.Property(x => x.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50)
                .HasDefaultValue(CircleSlotStatus.Vacant);

            builder.Property(x => x.VacatedAt)
                .IsRequired(false);

            builder.Property(x => x.AssignedAt)
                .IsRequired(false);

            builder.HasIndex(x => new { x.CircleId, x.SlotNumber })
                .IsUnique();

            builder.HasIndex(x => x.MemberLedgerId)
                .IsUnique()
                .HasFilter("[MemberLedgerId] IS NOT NULL");

            builder.HasIndex(x => x.Status);

            builder.HasOne(x => x.Circle)
                .WithMany(x => x.CircleSlots)
                .HasForeignKey(x => x.CircleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Optional because a vacant slot has no member ledger yet.
            builder.HasOne(x => x.MemberLedger)
                .WithOne(x => x.CircleSlot)
                .HasForeignKey<CircleSlot>(x => x.MemberLedgerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
