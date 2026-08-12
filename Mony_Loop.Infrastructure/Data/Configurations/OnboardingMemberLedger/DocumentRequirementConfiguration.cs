using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mony_Loop.Domain.Entities.Onboarding___Member_Ledger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Infrastructure.Data.Configurations.OnboardingMemberLedger
{
    public class DocumentRequirementConfiguration : IEntityTypeConfiguration<DocumentRequirement>
    {
        public void Configure(EntityTypeBuilder<DocumentRequirement> builder)
        {

            builder.HasKey(x => x.DocumentRequirementId);

            builder.Property(x => x.DocumentName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            builder.Property(x => x.IsRequired)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.DisplayOrder)
                .IsRequired()
                .HasDefaultValue(0);

            // Relationships
            builder.HasMany(x => x.Documents)
                .WithOne(x => x.DocumentRequirement)
                .HasForeignKey(x => x.DocumentRequirementId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
