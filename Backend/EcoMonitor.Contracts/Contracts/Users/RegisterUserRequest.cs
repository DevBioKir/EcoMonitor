using EcoMonitor.Contracts.Contracts.BinPhoto;
using EcoMonitor.Contracts.Contracts.User;

namespace EcoMonitor.Contracts.Contracts.Users
{
    /// <summary>
    /// Переделать Request, потому что принимает меньше данных для регистрации пользователя
    /// </summary>
    /// <param name="Firstname"></param>
    /// <param name="Surname"></param>
    /// <param name="Email"></param>
    /// <param name="PasswordHash"></param>
    /// <param name="isLoginConfirmed"></param>
    /// <param name="RoleId"></param>
    /// <param name="Role"></param>
    /// <param name="CreatedAt"></param>
    /// <param name="LastLogindAt"></param>
    /// <param name="LockedUntil"></param>
    /// <param name="BinPhoto"></param>
    public record RegisterUserRequest( 
        string Firstname,
        string Surname,
        string Email,
        string PasswordHash);
}
