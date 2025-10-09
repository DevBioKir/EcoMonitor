using EcoMonitor.Core.Models.Users;

namespace EcoMonitor.DataAccess.Repositories.Users;

public interface IUserRoleRepository
{
    Task<UserRole> GetByNameASync(string name, CancellationToken cancellationToken = default);
    Task<UserRole> GetByIdASync(Guid id, CancellationToken cancellationToken = default);
}