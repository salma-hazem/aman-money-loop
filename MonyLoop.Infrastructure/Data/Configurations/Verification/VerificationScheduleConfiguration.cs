using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MonyLoop.Domain.Entities.Verification;

namespace MonyLoop.Infrastructure.Data.Configurations.Verification;


public class VerificationScheduleConfiguration : IEntityTypeConfiguration<VerificationSchedule>
{
    public void Configure(EntityTypeBuilder<VerificationSchedule> builder)
    {
        builder.HasKey(s => s.VerificationScheduleId);

        builder.Property(s => s.ApplicationId)
            .IsRequired();

        builder.Property(s => s.VerificationRoundId)
            .IsRequired();

        builder.Property(s => s.Date)
            .IsRequired();

        builder.Property(s => s.Time)
            .IsRequired();

        builder.Property(s => s.LocationLink)
            .HasMaxLength(2048)
            .IsRequired(false);

        builder.Property(s => s.VideoLink)
            .HasMaxLength(2048)
            .IsRequired(false);

        builder.Property(s => s.Status)
                    .HasConversion<string>()
                    .HasMaxLength(50)
                    .IsRequired();

        builder.HasOne(s => s.MembershipApplication)
            .WithMany(a => a.VerificationSchedules)
            .HasForeignKey(s => s.ApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.VerificationRound)
            .WithMany(r => r.VerificationSchedules)
            .HasForeignKey(s => s.VerificationRoundId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
