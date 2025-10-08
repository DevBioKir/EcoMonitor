using EcoMonitor.Core.Models.Users;
using EcoMonitor.DataAccess.Entities.Users;

namespace EcoMonitor.DataAccess.Entities.Auth;

public class RefreshTokenEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public UserEntity User { get; set; } //navigational property
    public string TokenHash {get; set;}
    public DateTime IssuedAt { get; set; }
    public DateTime ExpireAt { get; set; }
    public bool Revoked  { get; set; }
}