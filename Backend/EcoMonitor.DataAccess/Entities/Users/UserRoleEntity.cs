using EcoMonitor.Core.ValueObjects;

namespace EcoMonitor.DataAccess.Entities.Users
{
    public class UserRoleEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public ICollection<UserEntity> Users = new List<UserEntity>();

        public ICollection<PermissionEntity> Permissions = new List<PermissionEntity>();
    }
}