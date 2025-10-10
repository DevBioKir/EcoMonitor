using EcoMonitor.Core.Models.Users;
using EcoMonitor.Core.ValueObjects;

namespace EcoMonitor.App.Factory.Users;

public interface IUserRoleFactory
{
    UserRole Restore(Guid id, string name, string description, IEnumerable<Permission> permissions);
}