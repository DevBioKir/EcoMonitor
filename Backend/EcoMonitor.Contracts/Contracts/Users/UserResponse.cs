using EcoMonitor.Contracts.Contracts.BinPhoto;

namespace EcoMonitor.Contracts.Contracts.User
{
    public record UserResponse(
        //Guid Id,
        string Firstname,
        string Surname,
        string Email,
        //string PasswordHash,
        //bool isLoginConfirmed,
        //Guid RoleId,
        //UserRoleResponse Role,
        //DateTime CreatedAt,
        //DateTime LastLogindAt,
        //DateTime LockedUntil,
        List<BinPhotoResponse> BinPhoto);
}
