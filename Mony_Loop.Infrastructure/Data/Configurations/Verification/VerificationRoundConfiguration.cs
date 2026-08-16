using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mony_Loop.Domain.Entities.Verification;

namespace MonyLoop.Infrastructure.Data.Configurations.Verification;


public class VerificationRoundConfiguration : IEntityTypeConfiguration<VerificationRound>
{
    public void Configure(EntityTypeBuilder<VerificationRound> builder)
    {
        builder.HasKey(r => r.VerificationRoundId);

        builder.Property(r => r.CircleId)
            .IsRequired();

        builder.Property(r => r.ReviewedByUserId)
            .IsRequired();

        builder.Property(r => r.RoundName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(r => r.Format)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.HasOne(r => r.Circle)
            .WithMany(c => c.VerificationRounds)
            .HasForeignKey(r => r.CircleId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
