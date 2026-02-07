using EcoMonitor.AdminPanel.Infrastucture.TokenStorage;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.JSInterop;

namespace EcoMonitor.AdminPanel.Infrastucture;

public class TokenCircuitHandler : CircuitHandler
{
    private readonly ITokenStore _tokenStore; 
    private readonly IJSRuntime _js;

    public TokenCircuitHandler(ITokenStore tokenStore, IJSRuntime js)
    {
        _tokenStore = tokenStore; _js = js;
    }

    public override async Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        Console.WriteLine("[Circuit] Loading tokens for new circuit..."); 
        await _tokenStore.LoadFromLocalStorageAsync(_js);
    }
}