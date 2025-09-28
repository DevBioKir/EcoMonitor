using EcoMonitor.Core.ValueObjects;

namespace EcoMonitor.App.Services.Authorization;

public class AuthorizationService : IAuthorizationService
{
    public void CheckPermisson(Core.Models.Users.User user, Permission permission)
    {
        if (!user.HasPermission(permission))
            throw new UnauthorizedAccessException($"Insufficient permission: {permission}");
    }
}