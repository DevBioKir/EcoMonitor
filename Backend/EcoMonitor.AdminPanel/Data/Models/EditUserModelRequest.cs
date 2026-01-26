using EcoMonitor.Contracts.Contracts.User;

namespace EcoMonitor.AdminPanel.Data.Models;

public sealed record EditUserModelRequest
{
    public Guid Id { get; set; }
    public string Firstname { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Guid RoleId { get; set; } = Guid.Empty;
    public UserRoleResponse Role { get; set; }
}