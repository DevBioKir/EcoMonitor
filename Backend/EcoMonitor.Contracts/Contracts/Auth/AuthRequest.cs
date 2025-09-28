namespace EcoMonitor.Contracts.Contracts.Auth;

public record AuthRequest(
    string Email,
    string Password);