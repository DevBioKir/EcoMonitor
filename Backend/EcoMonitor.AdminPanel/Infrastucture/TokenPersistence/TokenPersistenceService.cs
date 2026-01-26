using EcoMonitor.AdminPanel.Infrastucture.TokenStorage;
using Microsoft.JSInterop;

namespace EcoMonitor.AdminPanel.Infrastucture.TokenPersistence;

public class TokenPersistenceService : ITokenPersistenceService
{
    // private readonly ITokenStore _store;
    // private readonly IJSRuntime _js;
    private readonly IServiceProvider _provider;

    public TokenPersistenceService(IServiceProvider provider)
    {
        _provider = provider;
        // _store = store;
        // _js = js;
    }

    public async Task<(string access, string refresh)> LoadTokensAsync()
    {
        var js = _provider.GetRequiredService<IJSRuntime>();
        var access = await js.InvokeAsync<string>("localStorage.getItem", "accessToken");
        var refresh = await js.InvokeAsync<string>("localStorage.getItem", "refreshToken");
        return (access, refresh);
    }

    public async Task SaveAsync()
    {
        var _js = _provider.GetService<IJSRuntime>();
        var store = _provider.GetService<ITokenStore>();
        
        if (!store.HasAccessToken()) return;

        await _js.InvokeVoidAsync("localStorage.setItem", "accessToken", store.AccessToken);
        await _js.InvokeVoidAsync("localStorage.setItem", "refreshToken", store.RefreshToken);
    }

    public async Task ClearAsync()
    {
        var _js = _provider.GetService<IJSRuntime>();
        var store = _provider.GetService<ITokenStore>();
        
        await _js.InvokeVoidAsync("localStorage.removeItem", "accessToken");
        await _js.InvokeVoidAsync("localStorage.removeItem", "refreshToken");
        store.ClearTokens();
    }
}