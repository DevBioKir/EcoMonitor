namespace EcoMonitor.AdminPanel.Data.Models;

public sealed record CreateUserModelRequest
{
    public string Firstname { get; set; } = "";
    public string Surname { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}