using EcoMonitor.Contracts.Contracts.User;

namespace EcoMonitor.AdminPanel.Data.Models;

public sealed record UserModelResponse(
    Guid Id,
    string Firstname,
    string Surname,
    string Email,
    //string RoleName
    //bool isLoginConfirmed,
    UserRoleResponse RoleUser
    //DateTime CreatedAt,
    //DateTime LastLogindAt,
    //DateTime LockedUntil,
    //List<BinPhotoResponse> BinPhoto
    );