using EcoMonitor.Contracts.Contracts.BinPhoto;

namespace EcoMonitor.Contracts.Contracts.User
{
    public record UserWithPhotosResponse(
        Guid Id,
        string Firstname,
        string Surname,
        string Email,
        //bool isLoginConfirmed,
        //UserRoleResponse Role,
        //DateTime CreatedAt,
        //DateTime LastLogindAt,
        //DateTime LockedUntil,
        List<BinPhotoResponse> BinPhoto);
}
