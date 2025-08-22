using Microsoft.AspNet.Identity;

namespace EcoMonitor.Core.Models
{
    public class User
    {
        // Identity
        public Guid Id { get; private set; }
        public string Firstname { get; private set; } = string.Empty;
        public string Surname { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;

        // Security
        public string PasswordHash { get; private set; } = string.Empty;
        public string Salt { get; private set; } = string.Empty;
        public bool isLoginConfirmed { get; private set; }

        // Access control
        private readonly List<UserRole> _roles = new List<UserRole>();
        public IReadOnlyCollection<UserRole> Roles => _roles.AsReadOnly();

        // Auditing
        public DateTime CreatedAt { get; private set; }
        public DateTime LastLogindAt { get; private set; }
        public DateTime LockedUntil { get; private set; }

        private User() {}

        public static User Create(
            string firstname, 
            string surname, 
            string email, 
            string password, 
            IPasswordHasher hasher)
        {
            var (hash, salt) = hasher.HashPassword(password);

        }

    }
}
