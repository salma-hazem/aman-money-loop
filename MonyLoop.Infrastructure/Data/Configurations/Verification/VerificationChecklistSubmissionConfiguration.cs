using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MonyLoop.Domain.Entities.Verification;

namespace MonyLoop.Infrastructure.Data.Configurations.Verification;

public class VerificationChecklistSubmissionConfiguration : IEntityTypeConfiguration<VerificationChecklistSubmission>
{
    public void Configure(EntityTypeBuilder<VerificationChecklistSubmission> builder)
    {
        builder.HasKey(s => s.VerificationChecklistSubmissionId);

        builder.Property(s => s.VerificationScheduleId)
            .IsRequired();

        builder.Property(s => s.SubmittedByUserId)
            .IsRequired();

        builder.Property(s => s.CompositeScore)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(s => s.OverallComments)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(s => s.SubmittedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();

        builder.HasOne(s => s.VerificationSchedule)
            .WithMany(v => v.VerificationChecklistSubmissions)
            .HasForeignKey(s => s.VerificationScheduleId)
            .OnDelete(DeleteBehavior.Restrict);


        builder.HasMany(s => s.VerificationCriterionRatings)
            .WithOne(r => r.VerificationChecklistSubmission)
            .HasForeignKey(r => r.VerificationChecklistSubmissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
