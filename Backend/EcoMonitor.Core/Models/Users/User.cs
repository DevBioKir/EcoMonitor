using EcoMonitor.Core.Models.Auth;
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
        public bool AccountEnabled { get; private set; } = true;
        public string? BlockReason {get; private set;} 
        public DateTime? LockedUntil { get; private set; }
        
        
        private readonly List<RefreshToken> _refreshTokens = new();
        public IReadOnlyCollection<RefreshToken> RefreshTokens =>  _refreshTokens.AsReadOnly();
        
        private readonly List<BinPhoto> _photos = new();
        public IReadOnlyCollection<BinPhoto> Photos => _photos.AsReadOnly();

        private User() {}

        private User(
            string firstname,
            string surname,
            Email email,
            PasswordHash passwordHash,
            Guid roleId
            )
        {
            Id = Guid.NewGuid();
            Firstname = firstname;
            Surname = surname;
            Email = email;
            PasswordHash = passwordHash;
            isLoginConfirmed = true;
            //Role = role ?? throw new ArgumentNullException(nameof(role));
            RoleId = roleId;
            CreatedAt = DateTime.UtcNow;
            LastLogindAt = DateTime.UtcNow;

            Validate();
        }
        private User(
            Guid id,
            string firstname,
            string surname,
            Email email,
            PasswordHash passwordHash,
            UserRole role,
            //bool _isLoginConfirmed,
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
            //isLoginConfirmed = true;
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
            PasswordHash passwordHash,
            Guid roleId)
        {
            var emailVo = Email.Create(email);

            var user = new User(firstname, surname, emailVo, passwordHash, roleId);
            return user;

        }

        public static User Restore(
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
            List<BinPhoto> photos)
        {
            return new User (
                id, firstname, surname, email, passwordHash, role, createdAt, lastLogindAt, lockedUntil, photos);
        }

        public bool CheckPassword(string plainPassword, IPasswordHasher hasher) => 
            PasswordHash.Verify(plainPassword, hasher);

        public void SetRole(UserRole newRole, User currentUser)
        {
            if (newRole == null) throw new ArgumentNullException(nameof(newRole));
            if (currentUser == null) throw new ArgumentNullException(nameof(currentUser));

            if(!currentUser.HasPermission(Permission.RolesManage))
                throw new InvalidOperationException("User does not have permission to change roles.");

            Role = newRole;
            RoleId = newRole.Id;
        }

        public bool HasPermission(Permission permission) => Role.HasPermission(permission);
        public void UpdateFirstname(string newFirstname) => Firstname = newFirstname;
        public void UpdateSurname(string newSurname) => Surname = newSurname;
        public void UpdateEmail(string newEmail) => Email = Email.Create(newEmail);

        public void ChangeRole(UserRole newRole)
        {
            Role = newRole;
            RoleId = newRole.Id;
        }
        
        public void UpdateRole(UserRole newRole) => Role = newRole;
        public void UpdateLastLoggedAt(DateTime newLastLoggedAt) => LastLogindAt = newLastLoggedAt;

        public void ChangePassword(string currentPassword, string newPassword, IPasswordHasher hasher)
        {
            if(!CheckPassword(currentPassword, hasher))
                throw new UnauthorizedAccessException("Current password is incorrect");
            
            var newHash = PasswordHash.FromPlainPassword(currentPassword, hasher);
            PasswordHash =  newHash;
        }
        public void AddRefreshToken(RefreshToken token) => _refreshTokens.Add(token);

        public void RevokeRefreshToken(string tokenHash)
        {
            var token = _refreshTokens.FirstOrDefault(r => r.TokenHash == tokenHash);
            
            if (token != null)
            {
                token.Revoke();
            }
        }
        
        public bool HasValidRefreshToken(string tokenHash) 
            => _refreshTokens.Any(r => r.TokenHash == tokenHash && r.IsActive());

        public void BlockUser(string reason, TimeSpan? duration = null)
        {
            AccountEnabled = false;
            BlockReason = reason;
            LockedUntil = duration.HasValue ? DateTime.UtcNow + duration.Value : null;
        }

        public void UnlockAccount()
        {
            AccountEnabled = true;
            BlockReason = null;
        }
        
        public bool CanLogin() => AccountEnabled && isLoginConfirmed 
                                                 && (LockedUntil == null || LockedUntil < DateTime.UtcNow);
    }
}
