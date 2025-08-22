
namespace EcoMonitor.Core.Models
{
    public class UserRole
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;

        private readonly List<User> _users = new List<User>();
        public IReadOnlyCollection<User> Users => _users.AsReadOnly();
        private UserRole() {}

        public static UserRole Create(string name, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(name)) 
                throw new ArgumentNullException("Role name cannot be empty", nameof(name));
            return new UserRole
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description ?? string.Empty
            };
        }
        
        public void AssignUser(User user)
        {
            if (!_users.Contains(user))
                _users.Add(user);
        }

        public void RemoveUser(User user)
        {
            _users.Remove(user);
        }
    }
}
