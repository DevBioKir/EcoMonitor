using System.Net.Http.Headers;
using EcoMonitor.App.Services;
using EcoMonitor.Contracts.Contracts.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace EcoMonitor.AdminPanel.Infrastucture.Http;

public class AdminAuthHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AdminAuthHandler> _logger;
    private readonly IJSRuntime _JS = default!;
    private readonly IHttpClientFactory _httpClientFactory;
    private HttpClient _httpClient => _httpClientFactory.CreateClient("PublicApi");

    public AdminAuthHandler(
        IHttpContextAccessor httpContextAccessor, 
        ILogger<AdminAuthHandler> logger,
        IJSRuntime JS)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _JS = JS;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, 
        CancellationToken cancellationToken)
    {
        var token = GetAccessToken();
        
        _logger.LogInformation("AdminAuthHandler: {Method} {Url}", request.Method, request.RequestUri);
        _logger.LogInformation("Token from cookie: '{Token}'", token ?? "<null>");

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        _logger.LogInformation("Authorization header: {Auth}",
            request.Headers.Authorization?.ToString() ?? "<null>");

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            _logger.LogWarning("Received 401, attempting token refresh");
            
            try
            {
                await RefreshTokenAsync();
                
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", GetAccessToken());
                response = await base.SendAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token refresh failed");
                await RedirestToLoginAsync();
            }
        }
        return response;
    }
    
    private string GetAccessToken() => 
        _httpContextAccessor.HttpContext?.Request.Cookies["accessToken"] 
        ?? GetTokenFromLocalStorageAsync().GetAwaiter().GetResult();

    private async Task RefreshTokenAsync()
    {
        var refreshToken = await GetTokenFromLocalStorageAsync();

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new UnauthorizedAccessException("No refresh token");
        }

        var response = await _httpClient.PostAsJsonAsync(
            "/api/public/v1/Authorization/refresh-token", refreshToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new UnauthorizedAccessException("No refresh token");
        }
        
        var newToken = await response.Content.ReadFromJsonAsync<AuthResponse>();
        
        await _JS.InvokeVoidAsync("localStorage.setItem", "accessToken", newToken.AccessToken);
        await _JS.InvokeVoidAsync("localStorage.setItem", "refreshToken", newToken.RefreshToken);
        
        _httpContextAccessor.HttpContext?.Response.Cookies.Append("accessToken", newToken.AccessToken);
        _httpContextAccessor.HttpContext?.Response.Cookies.Append("refreshToken", newToken.RefreshToken);
    }

    private async Task RedirestToLoginAsync()
    {
        await _JS.InvokeVoidAsync("window.location.href", "/auth/login");
    }

    private async Task<string> GetTokenFromLocalStorageAsync()
    {
        return await _JS.InvokeAsync<string>("localStorage.getItem", "accessToken");
    }
    
    private async Task<string> GetRefreshTokenFromStorageAsync()
    {
        return await _JS.InvokeAsync<string>("localStorage.getItem", "refreshToken");
    }
}