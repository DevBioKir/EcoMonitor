using EcoMonitor.Contracts.Contracts.Auth;
using EcoMonitor.Contracts.Contracts.Users;
using EcoMonitor.Core.ValueObjects;
using Microsoft.AspNetCore.Identity.Data;

namespace EcoMonitor.App.Services;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(AuthRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken = default);
    Task ChangePassword(Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);
}