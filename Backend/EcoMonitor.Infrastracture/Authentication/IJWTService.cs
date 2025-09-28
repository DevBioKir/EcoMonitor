using EcoMonitor.Core.Models.Users;

namespace EcoMonitor.Infrastracture.Authentication
{
    public interface IJWTService
    {
        string GenerateToken(User user);
    }
}
