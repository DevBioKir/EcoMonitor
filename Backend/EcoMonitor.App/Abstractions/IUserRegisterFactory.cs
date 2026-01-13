using EcoMonitor.Core.Models.Users;

namespace EcoMonitor.App.Abstractions;

public interface IUserRegisterFactory
{
    UserRole Role { get; }
    User CreateUser(
        string firstname,
        string surname, 
        string email, 
        string password);
}