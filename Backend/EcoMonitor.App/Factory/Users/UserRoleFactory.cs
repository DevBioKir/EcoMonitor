using EcoMonitor.App.Abstractions;
using EcoMonitor.Core.Models.Users;
using EcoMonitor.Core.ValueObjects;

namespace EcoMonitor.App.Factory.Users;

public class UserRoleFactory : IUserRoleFactory
{
    public UserRole Restore(
        Guid id,
        string name,
        string description,
        IEnumerable<Permission> permissions)
    {
        return UserRole.Restore(id, name, description, permissions);
    }
}