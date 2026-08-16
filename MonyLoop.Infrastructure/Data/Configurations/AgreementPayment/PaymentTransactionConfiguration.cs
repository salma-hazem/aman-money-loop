using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MonyLoop.Domain.Constants.Agreement___Payment;
using MonyLoop.Domain.Entities.Agreement___Payment;

namespace MonyLoop.Infrastructure.Data.Configurations.AgreementPayment
{
    public class PaymentTransactionConfiguration
        : IEntityTypeConfiguration<PaymentTransaction>
    {
        public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
        {
            builder.ToTable("PaymentTransactions");

            builder.HasKey(x => x.PaymentTransactionId);

            builder.Property(x => x.MemberLedgerId)
                .IsRequired();

            builder.Property(x => x.CircleId)
                .IsRequired();

            builder.Property(x => x.RecordedByUserId)
                .IsRequired();

            builder.Property(x => x.TransactionType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(x => x.PaymentMethod)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(x => x.TransactionStatus)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(x => x.Amount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(x => x.TransactionReference)
                .IsRequired(false)
                .HasMaxLength(100);

            builder.Property(x => x.ReceiptNumber)
                .IsRequired(false)
                .HasMaxLength(100);

            builder.Property(x => x.ReceiptFilePath)
                .IsRequired(false)
                .HasMaxLength(500);

            builder.Property(x => x.TransactionDate)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            // MemberLedger 1 ---- Many PaymentTransactions
            builder.HasOne(x => x.MemberLedger)
                .WithMany(x => x.PaymentTransactions)
                .HasForeignKey(x => x.MemberLedgerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Circle 1 ---- Many PaymentTransactions
            builder.HasOne(x => x.Circle)
                .WithMany(x => x.PaymentTransactions)
                .HasForeignKey(x => x.CircleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
