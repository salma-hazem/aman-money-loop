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
    public class DocumentConfiguration : IEntityTypeConfiguration<Document>
    {
        public void Configure(EntityTypeBuilder<Document> builder)
        {
            builder.HasKey(x => x.DocumentId);

            builder.Property(x => x.FilePath)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasConversion<string>();

            builder.Property(x => x.RejectionReason)
                .HasMaxLength(500);

            builder.Property(x => x.UploadedAt)
                .IsRequired();

            builder.Property(x => x.ReviewedAt);
            builder.Property(x => x.ReviewedByUserId);

            // Relationships

            builder.HasOne(d => d.OnboardingCase)
                .WithMany(oc => oc.Documents)
                .HasForeignKey(d => d.OnboardingCaseId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);


        }
    }
}
