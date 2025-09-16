using EcoMonitor.Core.Models;
using EcoMonitor.Core.Models.Users;
using EcoMonitor.Core.ValueObjects;

namespace EcoMonitor.App.Abstractions
{
    public interface IUserFactory
    {
        User Create(string firstname, string surname, string email, string password);
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
    }
}