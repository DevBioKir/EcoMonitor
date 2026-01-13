using EcoMonitor.Contracts.Contracts.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;

namespace EcoMonitor.AdminPanel.Components.Pages.auth;

public partial class Login : ComponentBase
{
    [Inject] private IHttpClientFactory HttpClientFactory { get; set; } = default!;
    private HttpClient HttpClient => HttpClientFactory.CreateClient("PublicApi");
    
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private IHttpContextAccessor HttpContextAccessor { get; set; } = default!;

    private string email = "";
    private string password = "";
    private string error = "";
    private bool isLoading = false;

    private async Task LoginAsync()
    {
        error = "";
        isLoading = true;
        StateHasChanged();

        try
        {
            var authRequest = new AuthRequest(email, password);
            var response = await HttpClient.PostAsJsonAsync(
                "api/admin/v1/AdminAuthorization/admin-login", authRequest);

            if (!response.IsSuccessStatusCode)
            {
                error = "Невереный логин или пароль";
                return;
            }

            var loginResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
            var token = loginResponse?.AccessToken;

            if (string.IsNullOrWhiteSpace(token))
            {
                error = "Пустой токен от сервера";
                return;
            }

            var httpContext = HttpContextAccessor.HttpContext;
            if (httpContext is null)
            {
                error = "HttpContext is null";
                return;
            }
            
            httpContext.Response.Cookies.Append(
                "accessToken",
                token,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,              // в dev на http
                    SameSite = SameSiteMode.Lax,
                    Path = "/"                  // чтобы была доступна везде
                });

            // после установки куки — навигация
            Navigation.NavigateTo("/admin/dashboard", forceLoad: true);
        }
        catch (Exception ex)
        {
            error = $"Ошибка {ex.Message}";
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }
}