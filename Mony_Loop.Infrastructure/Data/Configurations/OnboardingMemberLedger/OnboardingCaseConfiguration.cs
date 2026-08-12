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
    public class OnboardingCaseConfiguration : IEntityTypeConfiguration<OnboardingCase>
    {
        public void Configure(EntityTypeBuilder<OnboardingCase> builder)
        {
            builder.HasKey(x => x.OnboardingCaseId);

            builder.Property(x => x.FinalStatus)
                .IsRequired()
                .HasMaxLength(50)
                .HasConversion<string>();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            //  Relationships

            builder.HasOne(x => x.MembershipAgreement)
                .WithMany()
                .HasForeignKey(x => x.MembershipAgreementId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.MemberLedger)
                .WithOne(x => x.OnboardingCase)
                .HasForeignKey<MemberLedger>(x => x.OnboardingCaseId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
