using EcoMonitor.App.Abstractions;
using EcoMonitor.Core.Models;
using EcoMonitor.Core.Models.Users;
using EcoMonitor.Core.ValueObjects;
using EcoMonitor.Infrastracture.Abstractions;

namespace EcoMonitor.App.Factory.Users
{
    public class UserFactory : IUserFactory
    {
        private readonly IPasswordHasher _passwordHasher;

        public UserFactory(IPasswordHasher passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }

        public User Create(string firstname, string surname, string email, string password)
        {
            var passwordHash = PasswordHash.FromPlainPassword(password, _passwordHasher);

            return User.Create(firstname, surname, email, passwordHash);
        }

        public User Restore(Guid id,
            string firstname,
            string surname,
            Email email,
            PasswordHash passwordHash,
            UserRole role,
            DateTime createdAt,
            DateTime lastLogindAt,
            DateTime lockedUntil,
            List<BinPhoto> photos)
        {
            return User.Restore(id, firstname, surname, email, passwordHash, role, createdAt, lastLogindAt, lockedUntil, photos);
        }
    }
}
