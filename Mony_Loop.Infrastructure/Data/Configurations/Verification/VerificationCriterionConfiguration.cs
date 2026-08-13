using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mony_Loop.Domain.Entities.Verification;

<<<<<<< HEAD
namespace MonyLoop.Infrastructure.Data.Configurations.Verification;
=======
namespace Mony_Loop.Infrastructure.Data.Configurations.Verification;
>>>>>>> 4e8705a (module-1/UserAuth)

public class VerificationCriterionConfiguration : IEntityTypeConfiguration<VerificationCriterion>
{
    public void Configure(EntityTypeBuilder<VerificationCriterion> builder)
    {
        builder.HasKey(c => c.VerificationCriterionId);

        builder.Property(c => c.VerificationRoundId)
            .IsRequired();

        builder.Property(c => c.CriterionName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.Weight)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(c => c.DisplayOrder)
            .IsRequired();

        builder.Property(c => c.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.HasOne(c => c.VerificationRound)
            .WithMany(r => r.VerificationCriteria)
            .HasForeignKey(c => c.VerificationRoundId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.VerificationCriterionRatings)
            .WithOne(r => r.VerificationCriterion)
            .HasForeignKey(r => r.VerificationCriterionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
