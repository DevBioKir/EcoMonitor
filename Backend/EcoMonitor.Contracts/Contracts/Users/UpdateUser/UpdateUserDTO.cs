using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using EcoMonitor.Contracts.Contracts.User;

namespace EcoMonitor.Contracts.Contracts.Users.UpdateUser;

public class UpdateUserDTO
{
    [JsonPropertyName("Firstname")]
    public string Firstname { get; set; }
    
    [JsonPropertyName("Surname")] 
    public string Surname { get; set; }
    
    [JsonPropertyName("Email")]
    [EmailAddress]
    public string Email { get; set; }
    
    [JsonPropertyName("UserRoleId")]
    public Guid UserRoleId { get; set; }
}