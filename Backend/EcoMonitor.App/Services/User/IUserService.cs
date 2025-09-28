using EcoMonitor.Contracts.Contracts.User;

namespace EcoMonitor.App.Services.User;

public interface IUserService
{
    Task<IReadOnlyList<UserResponse>> GetAllAsync(Guid currentUserId, CancellationToken cancellationToken = default);
    Task AddAsync(UserRequest user, Guid currentUser, CancellationToken cancellationToken = default);
    Task<UserResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserResponse> GetByEmailAsync(string email, Guid currentUserId, CancellationToken cancellationToken = default);
    Task<UserResponse> UpdateAsync(UserRequest user, Guid currentUserId, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, Guid currentUserId, CancellationToken cancellationToken = default);
}