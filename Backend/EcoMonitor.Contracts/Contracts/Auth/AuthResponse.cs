namespace EcoMonitor.Contracts.Contracts.Auth;

public record AuthResponse(
    string Token,
    int Expires);