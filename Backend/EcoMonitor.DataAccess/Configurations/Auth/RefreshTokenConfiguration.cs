using EcoMonitor.Core.Models.Users;
using EcoMonitor.DataAccess.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoMonitor.DataAccess.Configurations.Auth;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshTokenEntity>
{
    public void Configure(EntityTypeBuilder<RefreshTokenEntity> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.TokenHash)
            .IsRequired()
            .HasMaxLength(256);
        
        builder.Property(r => r.IssuedAt)
            .IsRequired();
        
        builder.Property(r => r.ExpireAt)
            .IsRequired();
        
        builder.Property(r => r.Revoked)
            .IsRequired();

        builder.HasIndex(r => new { r.UserId, r.TokenHash })
            .IsUnique();
        
        builder.HasOne(r=>r.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}