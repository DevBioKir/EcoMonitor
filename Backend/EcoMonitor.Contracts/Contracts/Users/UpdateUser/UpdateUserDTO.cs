using EcoMonitor.Contracts.Contracts.User;

namespace EcoMonitor.Contracts.Contracts.Users.UpdateUser;

public record UpdateUserDTO(
    string Firstname,
    string Surname,
    string Email,
    Guid UserRole);