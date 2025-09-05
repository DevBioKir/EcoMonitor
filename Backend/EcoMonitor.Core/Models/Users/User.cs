using EcoMonitor.Core.ValueObjects;
using EcoMonitor.Infrastracture.Abstractions;

namespace EcoMonitor.Core.Models.Users
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
        public bool isLoginConfirmed { get; private set; }

        // Access control
        public Guid RoleId { get; private set; }
        public UserRole Role { get; private set; } = null!;

        // Auditing
        public DateTime CreatedAt { get; private set; }
        public DateTime LastLogindAt { get; private set; }
        public DateTime LockedUntil { get; private set; }

        private readonly List<BinPhoto> _photos = new();
        public IReadOnlyCollection<BinPhoto> Photos => _photos.AsReadOnly();

        private User() {}

        private User(
            string firstname,
            string surname,
            Email email,
            PasswordHash passwordHash,
            UserRole role
            )
        {
            Id = Guid.NewGuid();
            Firstname = firstname;
            Surname = surname;
            Email = email;
            PasswordHash = passwordHash;
            Role = role ?? throw new ArgumentNullException(nameof(role));
            RoleId = role.Id;
            CreatedAt = DateTime.UtcNow;

            Validate();
        }
        private User(
            Guid id,
            string firstname,
            string surname,
            Email email,
            PasswordHash passwordHash,
            UserRole role,
            //bool isLoginConfirmed,
            DateTime createdAt,
            DateTime lastLogindAt,
            DateTime lockedUntil,
            List<BinPhoto> photos
            )
        {
            Id = id;
            Firstname = firstname;
            Surname = surname;
            Email = email;
            PasswordHash = passwordHash;
            Role = role;
            RoleId = role.Id;
            isLoginConfirmed = isLoginConfirmed;
            CreatedAt = createdAt;
            LastLogindAt = lastLogindAt;
            LockedUntil = lockedUntil;
            _photos = photos ?? new List<BinPhoto>();
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
            PasswordHash passwordHash)
            //IPasswordHasher hasher)
        {
            var emailVO = Email.Create(email);
            //var passwordHash = PasswordHash.FromPlainPassword(password, hasher);

            return new User(firstname, surname, emailVO, passwordHash, UserRole.User);
        }

        public static User Restore(
            Guid id,
            string firstname,
            string surname,
            string email,
            PasswordHash passwordHash,
            UserRole role,
            //bool isLoginConfirmed,
            DateTime createdAt,
            DateTime lastLogindAt,
            DateTime lockedUntil,
            List<BinPhoto> photos)
        {
            var emailVO = Email.Create(email);

            return new User (
                id, firstname, surname, emailVO, passwordHash, role, createdAt, lastLogindAt, lockedUntil, photos);
        }

        public bool CheckPassword(string plainPassword, IPasswordHasher hasher) => 
            PasswordHash.Verify(plainPassword, hasher);

        public void SetPole(UserRole newRole, User currentUser)
        {
            if(newRole == null) throw new ArgumentNullException(nameof(newRole));
            if (currentUser == null) throw new ArgumentNullException(nameof(currentUser));

            if(!currentUser.HasPermission(Permission.RolesManage))
                throw new InvalidOperationException("User does not have permission to change roles.");

            Role = newRole;
            RoleId = newRole.Id;
        }

        public bool HasPermission(Permission permission) => 
            Role.HasPermission(permission);
    }
}
