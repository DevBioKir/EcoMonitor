using EcoMonitor.Contracts.Contracts.Users.UpdateUser;
using EcoMonitor.Core.Models;
using EcoMonitor.Core.Models.Users;
using EcoMonitor.Core.ValueObjects;

namespace EcoMonitor.App.Abstractions
{
    public interface IUserFactory
    {
        //User Create(string firstname, string surname, string email, string password, Guid role);
        User UpdatePersonalInfo(User user, UpdatePersonalInfoRequest request);
        User UpdateEmail(User user, string email);
        User UpdateRole(User user, UserRole role);
        User Restore(Guid id,
            string firstname,
            string surname,
            Email email,
            PasswordHash passwordHash,
            UserRole role,
            DateTime createdAt,
            DateTime lastLogindAt,
            DateTime lockedUntil,
            List<BinPhoto> photos);

        public User RestoreBasic(
            Guid id,
            string firstname,
            string surname,
            Email email,
            PasswordHash passwordHash,
            UserRole role,
            DateTime createdAt,
            DateTime lastLogindAt,
            DateTime lockedUntil);
    }
}