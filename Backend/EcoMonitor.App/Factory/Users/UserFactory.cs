using EcoMonitor.App.Abstractions;
using EcoMonitor.Contracts.Contracts.Users.UpdateUser;
using EcoMonitor.Core.Models;
using EcoMonitor.Core.Models.Users;
using EcoMonitor.Core.ValueObjects;
using EcoMonitor.Infrastracture.Abstractions;

namespace EcoMonitor.App.Factory.Users
{
    public class UserFactory(IPasswordHasher passwordHasher) : IUserFactory
    {
        // public User Create(string firstname, string surname, string email, string password, Guid roleId)
        // {
        //     var passwordHash = PasswordHash.FromPlainPassword(password, passwordHasher);
        //
        //     return User.Create(firstname, surname, email, passwordHash, roleId);
        // }

        public User UpdatePersonalInfo(User user, UpdatePersonalInfoRequest request)
        {
            user.UpdatePersonalInfo(request.FirstName, request.Surname);
            return user;
        }
        
        public User UpdateEmail(User user, string email)
        {
            user.UpdateEmail(email);
            return user;
        }
        
        public User UpdateRole(User user, UserRole role)
        {
            user.ChangeRole(role);
            return user;
        }
        
        public User Restore(
            Guid id,
            string firstname,
            string surname,
            Email email,
            PasswordHash passwordHash,
            UserRole role,
            DateTime createdAt,
            DateTime lastLogindAt,
            DateTime lockedUntil,
            List<Core.Models.BinPhoto> photos)
        {
            return User.Restore(id, firstname, surname, email, passwordHash, role, createdAt, lastLogindAt, lockedUntil, photos);
        }
        
        public User RestoreBasic(
            Guid id,
            string firstname,
            string surname,
            Email email,
            PasswordHash passwordHash,
            UserRole role,
            DateTime createdAt,
            DateTime lastLogindAt,
            DateTime lockedUntil)
        {
            return User.Restore(
                id, 
                firstname, 
                surname, 
                email, 
                passwordHash, 
                role, 
                createdAt, 
                lastLogindAt, 
                lockedUntil, 
                new List<Core.Models.BinPhoto>());
        }
    }
}
