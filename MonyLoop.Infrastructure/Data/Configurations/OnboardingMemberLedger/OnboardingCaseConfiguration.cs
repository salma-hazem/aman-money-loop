using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MonyLoop.Domain.Entities.Onboarding___Member_Ledger;
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

            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            //  Relationships
            builder.HasOne(x => x.MembershipAgreement)
                .WithOne(x => x.OnboardingCase)
                .HasForeignKey<OnboardingCase>(x => x.MembershipAgreementId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.MemberLedger)
                .WithOne(x => x.OnboardingCase)
                .HasForeignKey<MemberLedger>(x => x.OnboardingCaseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Organizer)
                .WithMany()
                .HasForeignKey(x => x.OrganizerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
