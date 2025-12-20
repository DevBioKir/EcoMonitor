namespace EcoMonitor.AdminPanel.Data.Models;

public sealed record UserModelResponse(
    Guid Id,
    string Firstname,
    string Surname,
    string Email
    //bool isLoginConfirmed,
    //UserRoleResponse Role,
    //DateTime CreatedAt,
    //DateTime LastLogindAt,
    //DateTime LockedUntil,
    //List<BinPhotoResponse> BinPhoto
    );