using System.Net;
using System.Net.Http.Headers;
using EcoMonitor.AdminPanel.Infrastucture.TokenPersistence;
using EcoMonitor.AdminPanel.Infrastucture.TokenStorage;
using EcoMonitor.Contracts.Contracts.Auth;
using Microsoft.AspNetCore.Components;

namespace EcoMonitor.AdminPanel.Infrastucture.Http;

public class AdminAuthHandler : DelegatingHandler
{
    private readonly ITokenStore _tokens;
    private readonly IHttpClientFactory _factory;
    private readonly Http401HandlerAccessor _accessor;
    //private readonly NavigationManager _nav;
    private readonly ILogger<AdminAuthHandler> _logger;
    private readonly ITokenPersistenceService _persistenceService;
    private readonly IServiceProvider _serviceProvider;
    // private readonly ITokenStore _tokens;
    // private readonly IHttpContextAccessor _httpContextAccessor;
    // private readonly ILogger<AdminAuthHandler> _logger;
    // //private readonly IJSRuntime _JS = default!;
    // private readonly HttpClient _publicHttpClient;
    // //private HttpClient _httpClient => _httpClientFactory.CreateClient("PublicApi");

    public AdminAuthHandler(
        ITokenStore tokens,
        IHttpClientFactory factory,
        //NavigationManager nav,
        ILogger<AdminAuthHandler> logger, 
        IServiceProvider serviceProvider, ITokenPersistenceService persistenceService, Http401HandlerAccessor accessor)
    {
        _tokens = tokens;
        _factory = factory;
        //_nav = nav;
        _logger = logger;
        _serviceProvider = serviceProvider;
        _persistenceService = persistenceService;
        _accessor = accessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken ct)
    {
        _logger.LogInformation("SendAsync called for {Method} {Url}", request.Method, request.RequestUri);
        Console.WriteLine($"SendAsync called for {request.Method}, {request.RequestUri}");
        
        // if (!Components.App.TokensLoaded.Task.IsCompleted)
        // {
        //     _logger.LogInformation("Tokens not loaded yet, awaiting TokensLoaded task...");
        //     await Components.App.TokensLoaded.Task;
        //     _logger.LogInformation("TokensLoaded task completed");
        // }
        
        // int retries = 0;
        // while (!_tokens.HasAccessToken() && retries < 10)
        // {
        //     await Task.Delay(50); // ждём немного
        //     retries++;
        // }
        _logger.LogInformation("Checking if AccessToken exists...");
        if (!_tokens.HasAccessToken())
        {
            _logger.LogWarning("AccessToken empty, redirecting to login");
            //_nav.NavigateTo("/auth/login", true);
            // Возвращаем "пустой" response или новый 401
            return new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                RequestMessage = request
            };
        }
        
        _logger.LogInformation("AccessToken present, checking expiry...");
        if (_tokens.IsAccessTokenExpired())
        {
            _logger.LogInformation("AccessToken expired, trying refresh");

            var refreshed = await TryRefreshAsync();
            if (!refreshed)
            {
                _logger.LogWarning("Refresh token invalid, redirecting to login");
                _tokens.ClearTokens();
                return Unauthorized(request);
                //_nav.NavigateTo("/auth/login", true);

                // return new HttpResponseMessage(HttpStatusCode.Unauthorized)
                // {
                //     RequestMessage = request
                // };
            }
            _logger.LogInformation("AccessToken refreshed successfully");
        }
        
        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", _tokens.AccessToken);

        var response = await base.SendAsync(request, ct);

        // if (response.StatusCode != HttpStatusCode.Unauthorized)
        //     return response;
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            if (_tokens.HasRefreshToken() && await TryRefreshAsync())
            {
                _logger.LogInformation("Retrying request after refresh token...");
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", _tokens.AccessToken);
                _logger.LogInformation("Sending request with AccessToken: {Token}", _tokens.AccessToken[..10] + "...");

                return await base.SendAsync(request, ct);
            }

            _logger.LogWarning("Refresh failed or missing, clearing tokens and redirecting to login");
            _tokens.ClearTokens();
            return Unauthorized(request);
            
            // _tokens.ClearTokens();
            //_nav.NavigateTo("/auth/login", true);
        }
        return response;
    }

    private async Task<bool> TryRefreshAsync()
    {
        if (!_tokens.HasRefreshToken())
            return false;

        var client = _factory.CreateClient("PublicApi");

        var response = await client.PostAsJsonAsync(
            "/api/public/v1/Authorization/refresh-token",
            new { refreshToken = _tokens.RefreshToken });

        if (!response.IsSuccessStatusCode)
            return false;

        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        if (auth is null)
            return false;

        _tokens.SetTokens(auth.AccessToken, auth.RefreshToken);
        await _persistenceService.SaveAsync();
        
        return true;
    }

    private HttpResponseMessage Unauthorized(HttpRequestMessage request)
    {
        _accessor.Handler.TriggerRedirect();
        
        return new HttpResponseMessage(HttpStatusCode.Unauthorized)
        {
            RequestMessage = request
        };
    }
    
    // private string GetAccessToken() => 
    //     _httpContextAccessor.HttpContext?.Request.Cookies["accessToken"] 
    //     ?? GetTokenFromLocalStorageAsync().GetAwaiter().GetResult();
    //
    // private async Task RefreshTokenAsync()
    // {
    //     var refreshToken = await GetTokenFromLocalStorageAsync();
    //
    //     if (string.IsNullOrWhiteSpace(refreshToken))
    //     {
    //         throw new UnauthorizedAccessException("No refresh token");
    //     }
    //
    //     var response = await _httpClient.PostAsJsonAsync(
    //         "/api/public/v1/Authorization/refresh-token", refreshToken);
    //
    //     if (!response.IsSuccessStatusCode)
    //     {
    //         throw new UnauthorizedAccessException("No refresh token");
    //     }
    //     
    //     var newToken = await response.Content.ReadFromJsonAsync<AuthResponse>();
    //     
    //     await _JS.InvokeVoidAsync("localStorage.setItem", "accessToken", newToken.AccessToken);
    //     await _JS.InvokeVoidAsync("localStorage.setItem", "refreshToken", newToken.RefreshToken);
    //     
    //     _httpContextAccessor.HttpContext?.Response.Cookies.Append("accessToken", newToken.AccessToken);
    //     _httpContextAccessor.HttpContext?.Response.Cookies.Append("refreshToken", newToken.RefreshToken);
    // }
    //
    // private async Task RedirestToLoginAsync()
    // {
    //     var httpContext = _httpContextAccessor.HttpContext;
    //     string currentUrl = "/";
    //
    //     if (httpContext is not null)
    //     {
    //         currentUrl = httpContext.Request.Path + httpContext.Request.QueryString;
    //     }
    //     
    //     // Получаем текущий URL страницы
    //     var returnUrl = Uri.EscapeDataString(_httpContextAccessor.HttpContext?.Request.Path.Value ?? "/");
    //
    //     // Редирект на login с returnUrl
    //     await _JS.InvokeVoidAsync("window.location.href", $"/auth/login?returnUrl={returnUrl}");
    // }
    //
    // private async Task<string> GetTokenFromLocalStorageAsync()
    // {
    //     return await _JS.InvokeAsync<string>("localStorage.getItem", "accessToken");
    // }
    //
    // private async Task<string> GetRefreshTokenFromStorageAsync()
    // {
    //     return await _JS.InvokeAsync<string>("localStorage.getItem", "refreshToken");
    // }
}