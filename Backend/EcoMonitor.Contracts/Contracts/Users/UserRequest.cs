using EcoMonitor.Contracts.Contracts.BinPhoto;

namespace EcoMonitor.Contracts.Contracts.User
{
    public record UserRequest(
        Guid Id,
        string Firstname,
        string Surname,
        string Email,
        string Password,
        bool isLoginConfirmed,
        Guid RoleId,
        UserRoleRequest Role,
        DateTime CreatedAt,
        DateTime LastLogindAt,
        DateTime LockedUntil,
        List<BinPhotoRequest> BinPhoto);
}
