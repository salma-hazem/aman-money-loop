using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MonyLoop.Domain.Entities.UserAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Infrastructure.Data.Configurations.UserAuth
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(x => x.RefreshTokenId);

            builder.Property(x => x.Token)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.ExpiresAt).IsRequired();

            builder.Property(x => x.CreatedAt).IsRequired();

            builder.Property(x => x.IsRevoked).HasDefaultValue(false);

            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.Token).IsUnique();
        }
    }

}
