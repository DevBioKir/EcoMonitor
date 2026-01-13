using EcoMonitor.Contracts.Contracts.User;
using EcoMonitor.Contracts.Contracts.Users;
using EcoMonitor.Contracts.Contracts.Users.UpdateUser;

namespace EcoMonitor.App.Services.User;

public interface IUserService
{
    Task<IReadOnlyList<UserResponse>> GetAllAsync(Guid? currentUserId, CancellationToken cancellationToken = default);
    Task AddAsync(UserRequest user, Guid? currentUser, CancellationToken cancellationToken = default);
    Task<UserWithPhotosResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserWithPhotosResponse> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<UserResponse> UpdateAsync(
        Guid actorId, Guid userId, UpdateUserDTO request, CancellationToken cancellationToken = default);
    // Task<UserResponse> UpdatePersonalInfoAsync(
    //     Guid actorId, Guid userId, UpdatePersonalInfoRequest request, CancellationToken cancellationToken = default);
    // Task<UserResponse> UpdateEmailAsync(
    //     Guid actorId, Guid userId, UpdateEmailRequest request, CancellationToken cancellationToken = default);
    // Task<UserResponse> UpdateUserRoleAsync(
    //     Guid actorId, Guid userId, UpdateRoleRequest request, CancellationToken cancellationToken = default);
}