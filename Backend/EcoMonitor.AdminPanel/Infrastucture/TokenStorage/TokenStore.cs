using System.IdentityModel.Tokens.Jwt;
using Microsoft.JSInterop;

namespace EcoMonitor.AdminPanel.Infrastucture.TokenStorage;

public class TokenStore : ITokenStore
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    
    public event Action? OnTokensLoaded;

    public void RaiseTokensLoaded()
    {
        OnTokensLoaded?.Invoke();
    }

    public async Task LoadFromLocalStorageAsync(IJSRuntime js)
    {
        Console.WriteLine("TokenStore: loading tokens from localStorage..."); 
        AccessToken = await js.InvokeAsync<string>("localStorage.getItem", "accessToken"); 
        RefreshToken = await js.InvokeAsync<string>("localStorage.getItem", "refreshToken"); 
        
        Console.WriteLine("TokenStore loaded:"); Console.WriteLine("AccessToken: " + AccessToken); 
        Console.WriteLine("RefreshToken: " + RefreshToken); 
        OnTokensLoaded?.Invoke();
    }

    public bool HasAccessToken() => !string.IsNullOrWhiteSpace(AccessToken);

    public bool HasRefreshToken() => !string.IsNullOrWhiteSpace(RefreshToken);

    public void SetTokens(string accessToken, string refreshToken)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        OnTokensLoaded?.Invoke();
    }

    public void ClearTokens()
    {
        AccessToken = null;
        RefreshToken = null;
        OnTokensLoaded?.Invoke();
    }

    public bool IsAccessTokenExpired()
    {
        if (string.IsNullOrWhiteSpace(AccessToken))
        {
            return true;
        }

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(AccessToken);
            var exp = jwt.Payload.Exp;
            
            if (exp == null)
                return true;
            
            var expDate = DateTimeOffset.FromUnixTimeSeconds(exp.Value).UtcDateTime;
            return expDate <= DateTime.UtcNow;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}