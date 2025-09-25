using EcoMonitor.Contracts.Contracts.User;

namespace EcoMonitor.App.Services.User;

public interface IUserService
{
    Task<IReadOnlyList<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(UserRequest user, CancellationToken cancellationToken = default);
    Task<UserResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserResponse> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<UserResponse> UpdateAsync(UserRequest user, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}