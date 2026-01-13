using EcoMonitor.Contracts.Contracts.Auth;
using EcoMonitor.Contracts.Contracts.Users;
using EcoMonitor.Core.Models.Auth;
using EcoMonitor.Core.ValueObjects;
using Microsoft.AspNetCore.Identity.Data;

namespace EcoMonitor.App.Services;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(AuthRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> RegisterAsync(RegisterUserRequest request, string roleName, CancellationToken cancellationToken = default);
    // //Task<AuthResponse> RegisterAdminAsync(RegisterUserRequest request, CancellationToken cancellationToken = default);
    // Task<AuthResponse> RegisterManagerAsync(RegisterUserRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse>  ChangePasswordAsync(
        Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);
    Task<AuthResponse>  RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task RevokeRefreshTokenAsync(string refreshTokenValue, CancellationToken cancellationToken = default);
    Task BlockUserAsync(Guid id, string reason, TimeSpan duration, CancellationToken cancellationToken = default);
}