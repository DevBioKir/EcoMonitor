using EcoMonitor.Core.ValueObjects;

namespace EcoMonitor.Core.Models.Users
{
    public class UserRole
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;

        // Permission
        private readonly List<Permission> _permissions = new();
        public IReadOnlyCollection<Permission> Permissions => _permissions.AsReadOnly();

        //Users
        private readonly List<User> _users = new();
        public IReadOnlyCollection<User> Users => _users.AsReadOnly();

        private UserRole() {}
        private UserRole(
            string name, 
            string description, 
            IEnumerable<Permission>? permissions = null)
        {
            Id = Guid.NewGuid();
            Name = name;
            Description = description ?? string.Empty;

            if (permissions != null)
                _permissions.AddRange(permissions);
        }
        
        private UserRole(
            Guid id,
            string name, 
            string description, 
            IEnumerable<Permission>? permissions = null)
        {
            Id = Guid.NewGuid();
            Name = name;
            Description = description ?? string.Empty;

            if (permissions != null)
                _permissions.AddRange(permissions);
        }

        public static readonly UserRole Admin = new(
            "Admin",
            "Full access",
            new[]
            {
                Permission.UsersView,
                Permission.UsersAdd,
                Permission.UsersEdit,
                Permission.UsersDelete,
                Permission.RolesManage,
                Permission.PhotosView,
                Permission.PhotosAdd,
                Permission.PhotosEdit,
                Permission.PhotosDelete
            });

        public static readonly UserRole Manager = new (
            "Manager",
            "Manage photos and view/edit users",
            new[]
            {
                Permission.UsersView,
                Permission.UsersEdit,
                Permission.PhotosView,
                Permission.PhotosAdd,
                Permission.PhotosEdit,
                Permission.PhotosDelete
            });

        public static readonly UserRole User = new(
            "User",
            "Normal user access",
            new[]
            {
                Permission.UsersView,
                Permission.PhotosAdd,
                Permission.PhotosEdit,
            });

        public static UserRole Create(
            string name, 
            string? description = null,
            IEnumerable<Permission> permission = null)
        {
            if (string.IsNullOrWhiteSpace(name)) 
                throw new ArgumentNullException("Role name cannot be empty", nameof(name));

            return new UserRole(name, description, permission);
        }

        public static UserRole Restore(
            Guid id,
            string name,
            string? description = null,
            IEnumerable<Permission>? permissions = null)
        {
            return new UserRole(id, name, description, permissions);
        }

        internal void AddUser(User user)
        {
            if (!_users.Contains(user))
                _users.Add(user);
        }

        internal void RemoveUser(User user)
        {
            _users.Remove(user);
        }

        public bool HasPermission(Permission permission) => _permissions.Contains(permission);
    }
}
