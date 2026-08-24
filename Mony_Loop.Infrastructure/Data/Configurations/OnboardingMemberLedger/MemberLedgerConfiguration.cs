using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mony_Loop.Domain.Entities.Onboarding___Member_Ledger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mony_Loop.Infrastructure.Data.Configurations.OnboardingMemberLedger
{
    public class MemberLedgerConfiguration : IEntityTypeConfiguration<MemberLedger>
    {
        public void Configure(EntityTypeBuilder<MemberLedger> builder)
        {
            builder.HasKey(x => x.MemberLedgerId);

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.OnboardingCaseId)
                .IsRequired();

            builder.Property(x => x.ActivatedByAdminId)
                .IsRequired();

            builder.Property(x => x.ActivatedAt)
                .IsRequired();

            //  Relationships

            builder.HasOne(x => x.OnboardingCase)
                 .WithMany()
                 .HasForeignKey(x => x.OnboardingCaseId)
                 .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ActivatedByAdmin)
                .WithMany()
                .HasForeignKey(x => x.ActivatedByAdminId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.PaymentTransactions)
                .WithOne(x => x.MemberLedger)
                .HasForeignKey(x => x.MemberLedgerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
