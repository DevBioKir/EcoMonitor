using EcoMonitor.Core.ValueObjects;

namespace EcoMonitor.App.Services.Authorization;

public interface IAuthorizationService
{
    void CheckPermisson(Core.Models.Users.User user, Permission permission);
}