namespace EcoMonitor.Contracts.Contracts.Auth;

public record ChangePasswordRequest(
    string UserId,
    string CurrentPassword,
    string NewPassword);