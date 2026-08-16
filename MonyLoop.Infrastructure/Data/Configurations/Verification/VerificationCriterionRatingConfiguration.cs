using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MonyLoop.Domain.Entities.Verification;

namespace MonyLoop.Infrastructure.Data.Configurations.Verification;


public class VerificationCriterionRatingConfiguration : IEntityTypeConfiguration<VerificationCriterionRating>
{
    public void Configure(EntityTypeBuilder<VerificationCriterionRating> builder)
    {
        // Primary Key
        builder.HasKey(r => r.VerificationCriterionRatingId);

        // Foreign Key Properties
        builder.Property(r => r.VerificationChecklistSubmissionId)
            .IsRequired();

        builder.Property(r => r.VerificationCriterionId)
            .IsRequired();

        // Data Properties
        builder.Property(r => r.Rating)
            .IsRequired();

        builder.HasIndex(r => new
            {
                r.VerificationChecklistSubmissionId,
                r.VerificationCriterionId
            })
            .IsUnique();

        builder.ToTable(tableBuilder =>
            tableBuilder.HasCheckConstraint(
                "CK_VerificationCriterionRating_Rating",
                "[Rating] BETWEEN 1 AND 5"));

        builder.Property(r => r.Comments)
            .HasMaxLength(1000)
            .IsRequired(false);
    }
}
