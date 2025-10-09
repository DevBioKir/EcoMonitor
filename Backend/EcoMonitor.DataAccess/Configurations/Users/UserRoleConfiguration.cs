using EcoMonitor.Core.Models.Users;
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

            builder.HasData(
                new UserRoleEntity
                {
                    Id = RoleConstants.AdminId,
                    Name = "Admin",
                    Description = "Full access"
                },
                new UserRoleEntity
                {
                    Id = RoleConstants.ManagerId,
                    Name = "Manager",
                    Description = "Manage photos and view/edit users"
                },
                new UserRoleEntity
                {
                    Id = RoleConstants.UserId,
                    Name = "User",
                    Description = "Normal user access"
                }
            );
        }
    }
}
