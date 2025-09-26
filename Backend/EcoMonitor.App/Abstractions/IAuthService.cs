using EcoMonitor.Contracts.Contracts.Auth;
using EcoMonitor.Core.ValueObjects;
using Microsoft.AspNetCore.Identity.Data;

namespace EcoMonitor.App.Services;

public interface IAuthService
{
    Task<AuthResponse> Login(AuthRequest request, CancellationToken cancellationToken = default);
    Task ChangePassword(Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);
}