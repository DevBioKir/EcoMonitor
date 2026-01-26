using EcoMonitor.Contracts.Contracts.User;

namespace EcoMonitor.Contracts.Contracts.Users;

public record UserResponse(
    Guid Id,
    string Firstname,
    string Surname,
    string Email,
    UserRoleResponse RoleUser,
    string BlockReason,
    DateTime LockedUntil);