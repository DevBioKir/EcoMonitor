using EcoMonitor.Core.ValueObjects;
using Microsoft.AspNet.Identity;

namespace EcoMonitor.Core.Models
{
    public class User
    {
        // Identity
        public Guid Id { get; private set; }
        public string Firstname { get; private set; } = string.Empty;
        public string Surname { get; private set; } = string.Empty;
        public Email Email { get; private set; }

        // Security
        public PasswordHash PasswordHash { get; private set; }
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

        private User(
            string firstname,
            string surnave,
            string email,
            string passwordHash
            )
        {
            Id = Guid.NewGuid();
            Firstname = firstname;
            Surname = surnave;
            Email = new Email(email) ?? throw new ArgumentException(nameof(email));
            PasswordHash = PasswordHash.HashPassword(passwordHash);
            CreatedAt = DateTime.UtcNow;
            Validate();
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Firstname))
                throw new ArgumentException("Firstname required");
            if (string.IsNullOrWhiteSpace(Surname))
                throw new ArgumentException("Surname required");
        }

        public static User Create(
            string firstname,
            string surname,
            string email,
            string passwordHash)
        {
            return new User(
                firstname, 
                surname, 
                email, 
                passwordHash);
        }

    }
}
