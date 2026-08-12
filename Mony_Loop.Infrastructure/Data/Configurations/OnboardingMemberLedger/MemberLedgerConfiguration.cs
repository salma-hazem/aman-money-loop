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

            builder.HasOne(x => x.CircleSlot)
                .WithMany()
                .HasForeignKey(x => x.MemberLedgerId);

            builder.HasMany(x => x.PaymentTransactions)
                .WithOne(x => x.MemberLedger)
                .HasForeignKey(x => x.MemberLedgerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}