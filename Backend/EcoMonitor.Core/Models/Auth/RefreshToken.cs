using EcoMonitor.Core.Models.Users;

namespace EcoMonitor.Core.Models.Auth;

public class RefreshToken
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public User User { get; private set; } //navigational property
    public string TokenHash {get; private set;}
    public DateTime IssuedAt { get; private set; }
    public DateTime ExpireAt { get; private set; }
    public bool Revoked  { get; private set; }

    private RefreshToken(){}
        
    private RefreshToken(
        Guid userId,
        User user,
        string tokenHash,
        DateTime issuedAt,
        DateTime expireAt)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        User = user;
        TokenHash = tokenHash;
        IssuedAt = issuedAt;
        ExpireAt = expireAt;
        Revoked = false;
    }
    
    private RefreshToken(
        Guid id,
        Guid userId,
        User user,
        string tokenHash,
        DateTime issuedAt,
        DateTime expireAt,
        bool revoked)
    {
        Id = id;
        UserId = userId;
        User = user;
        TokenHash = tokenHash;
        IssuedAt = issuedAt;
        ExpireAt = expireAt;
        Revoked = revoked;
    }

    public static RefreshToken Create(
        Guid userId,
        User user,
        string tokenHash,
        int validDays = 30)
    {
        var now = DateTime.UtcNow;
        return new RefreshToken(
            userId,
            user,
            tokenHash,
            now,
            now.AddDays(validDays));
    }
    
    public static RefreshToken Restore(
        Guid id,
        Guid userId,
        User user,
        string tokenHash,
        DateTime issuedAt,
        DateTime expireAt,
        bool revoked)
    {
        return new RefreshToken(
            id,
            userId,
            user,
            tokenHash,
            issuedAt,
            expireAt,
            revoked);
    }

    public void Revoke()
    {
        Revoked = true;
    }

    public bool IsActive()
    {
        return !Revoked && DateTime.UtcNow < ExpireAt;
    }
}