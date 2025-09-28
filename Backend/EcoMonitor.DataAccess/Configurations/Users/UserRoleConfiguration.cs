using EcoMonitor.DataAccess.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoMonitor.DataAccess.Configurations.Users
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRoleEntity>
    {
        public void Configure(EntityTypeBuilder<UserRoleEntity> builder)
        {
            builder.HasKey(ur => ur.Id);

            builder.Property(ur => ur.Name)
                .IsRequired();

            builder.Property(ur => ur.Description)
                .HasMaxLength(100);
        }
    }
}
