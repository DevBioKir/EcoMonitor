using EcoMonitor.Core.Models.Users;

namespace EcoMonitor.DataAccess.Entities.Users
{
    public class UserEntity
    {
        public Guid Id { get; set; }
        public string Firstname { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Email { get; set; }

        public string PasswordHash { get; set; }
        public bool isLoginConfirmed { get; set; }

        public Guid RoleId { get; set; }
        public UserRoleEntity Role { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime LastLogindAt { get; set; }
        public DateTime LockedUntil { get; set; }

        public ICollection<BinPhotoEntity> BinPhoto = new List<BinPhotoEntity>();
    }
}
