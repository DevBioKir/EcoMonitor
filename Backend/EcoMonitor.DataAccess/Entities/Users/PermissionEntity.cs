namespace EcoMonitor.DataAccess.Entities.Users
{
    public class PermissionEntity
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public ICollection<UserRoleEntity> Roles { get; set; } = new List<UserRoleEntity>();
    }
}
