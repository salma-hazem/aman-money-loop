using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MonyLoop.Domain.Constants.Agreement___Payment;
using MonyLoop.Domain.Entities.Agreement___Payment;

namespace MonyLoop.Infrastructure.Data.Configurations.AgreementPayment
{
    public class MembershipAgreementConfiguration
        : IEntityTypeConfiguration<MembershipAgreement>
    {
        public void Configure(EntityTypeBuilder<MembershipAgreement> builder)
        {

            builder.ToTable("MembershipAgreements");

            builder.HasKey(x => x.MembershipAgreementId);

            builder.Property(x => x.MembershipApplicationId)
                .IsRequired();

            builder.Property(x => x.MemberName)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(x => x.CircleTitle)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(x => x.ContributionSchedule)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.PayoutSlot)
                .IsRequired();

            builder.Property(x => x.StartDate)
                .IsRequired()
                .HasColumnType("date");

            builder.Property(x => x.ExpiryDate)
                .IsRequired()
                .HasColumnType("date");

            builder.Property(x => x.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50)
                .HasDefaultValue(AgreementStatus.Pending);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.RespondedAt)
                .IsRequired(false);

            // MembershipApplication 1 ---- 0..1 MembershipAgreement
            builder.HasOne(x => x.MembershipApplication)
                .WithOne(x => x.MembershipAgreement)
                .HasForeignKey<MembershipAgreement>(
                    x => x.MembershipApplicationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
