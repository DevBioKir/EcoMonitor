namespace EcoMonitor.Contracts.Contracts.Auth;

public record AuthResponse(
    string AccessToken,
    string RefreshToken,
    int Expires);