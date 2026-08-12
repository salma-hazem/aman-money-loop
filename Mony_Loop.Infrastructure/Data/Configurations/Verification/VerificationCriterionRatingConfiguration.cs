using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mony_Loop.Domain.Entities.Verification;

<<<<<<< HEAD
namespace MonyLoop.Infrastructure.Data.Configurations.Verification;
=======
namespace Mony_Loop.Infrastructure.Data.Configurations.Verification;
>>>>>>> 4e8705a (module-1/UserAuth)

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

        builder.Property(r => r.Comments)
            .HasMaxLength(1000)
            .IsRequired(false);
    }
}
