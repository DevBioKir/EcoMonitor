namespace EcoMonitor.AdminPanel.Infrastucture.TokenStorage;

public interface ITokenStore
{
    string AccessToken { get; set; }
    string RefreshToken { get; set; }
    
    bool HasAccessToken();
    bool HasRefreshToken();
    
    void SetTokens(string accessToken, string refreshToken);
    
    void ClearTokens();
    
    bool IsAccessTokenExpired();
}