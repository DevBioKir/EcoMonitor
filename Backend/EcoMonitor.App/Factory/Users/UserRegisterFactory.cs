using EcoMonitor.App.Abstractions;
using EcoMonitor.Core.Models.Users;
using EcoMonitor.Core.ValueObjects;
using EcoMonitor.DataAccess.Repositories.Users;
using EcoMonitor.Infrastracture.Abstractions;

namespace EcoMonitor.App.Factory.Users;

public class UserRegisterFactory(IPasswordHasher passwordHasher) : IUserRegisterFactory
{
    public UserRole Role => UserRole.User;

    // public User CreateUser(string firstname, string surname, string email, string password)
    // {
    //     var passwordHash = PasswordHash.FromPlainPassword(password, passwordHasher);
    //     
    //     return User.Create(firstname, surname, email, passwordHash, Role);
    // }
    
    public User CreateUser(string firstname, string surname, string email, string password)
    {
        var passwordHash = PasswordHash.FromPlainPassword(password, passwordHasher);
        
        return User.Create(firstname, surname, email, passwordHash, Role.Id);
    }
}