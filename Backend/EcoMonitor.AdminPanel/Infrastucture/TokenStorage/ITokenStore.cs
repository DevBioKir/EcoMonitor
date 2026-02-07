using Microsoft.JSInterop;

namespace EcoMonitor.AdminPanel.Infrastucture.TokenStorage;

public interface ITokenStore
{
    string AccessToken { get; set; }
    string RefreshToken { get; set; }
    
    event Action? OnTokensLoaded;
    
    bool HasAccessToken();
    bool HasRefreshToken();
    
    void SetTokens(string accessToken, string refreshToken);
    
    void ClearTokens();
    
    bool IsAccessTokenExpired();
    Task LoadFromLocalStorageAsync(IJSRuntime js);
    void RaiseTokensLoaded();
}