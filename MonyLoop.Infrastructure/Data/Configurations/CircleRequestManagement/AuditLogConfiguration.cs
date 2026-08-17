using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MonyLoop.Domain.Entities.CircleRequestManagement;
using MonyLoop.Domain.Entities.UserAuth;

namespace MonyLoop.Infrastructure.Data.Configurations.CircleRequestManagement
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("AuditLogs");

            builder.HasKey(x => x.AuditLogId);

            builder.Property(x => x.EntityType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.EntityId)
                .IsRequired(false);

            builder.Property(x => x.ActionType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.PerformedByUserId)
                .IsRequired();

            builder.Property(x => x.OldStatus)
                .IsRequired(false)
                .HasMaxLength(50);

            builder.Property(x => x.NewStatus)
                .IsRequired(false)
                .HasMaxLength(50);

            builder.Property(x => x.ActionDescription)
                .IsRequired(false)
                .HasMaxLength(1000);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.HasIndex(x => x.EntityType);
            builder.HasIndex(x => x.EntityId);
            builder.HasIndex(x => x.PerformedByUserId);
            builder.HasIndex(x => x.CreatedAt);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.PerformedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
