using EcoMonitor.AdminPanel.Infrastucture.TokenStorage;
using EcoMonitor.Contracts.Contracts.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace EcoMonitor.AdminPanel.Components.Pages.auth;

public partial class Login : ComponentBase
{
    [Inject] private IHttpClientFactory HttpClientFactory { get; set; } = default!;
    private HttpClient HttpClient => HttpClientFactory.CreateClient("PublicApi");
    
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!;
    [Inject] private ITokenStore TokenStore { get; set; } = default!;
    //[Inject] private IHttpContextAccessor HttpContextAccessor { get; set; } = default!;
    [Parameter] public string ReturnUrl { get; set; } 

    private string email = "";
    private string password = "";
    private string error = "";
    private bool isLoading = false;

    protected async Task LoginAsync()
        {
            error = "";
            isLoading = true;
            StateHasChanged();

            try
            {
                var authRequest = new AuthRequest(email, password);
                await JS.InvokeVoidAsync("console.log", "[Login] Отправляем запрос на сервер", authRequest);

                var response = await HttpClient.PostAsJsonAsync(
                    "api/admin/v1/AdminAuthorization/admin-login", authRequest);

                await JS.InvokeVoidAsync("console.log", "[Login] Получен ответ", response.StatusCode);

                if (!response.IsSuccessStatusCode)
                {
                    error = "Неверный логин или пароль";
                    return;
                }

                var loginResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
                await JS.InvokeVoidAsync("console.log", "[Login] Ответ сервера", loginResponse);

                if (loginResponse is null || string.IsNullOrWhiteSpace(loginResponse.AccessToken))
                {
                    error = "Пустой токен от сервера";
                    return;
                }

                TokenStore.SetTokens(loginResponse.AccessToken, loginResponse.RefreshToken);

                await JS.InvokeVoidAsync("console.log", "[Login] TokenStore обновлен",
                    TokenStore.AccessToken, TokenStore.RefreshToken);

                await JS.InvokeVoidAsync("localStorage.setItem", "accessToken", loginResponse.AccessToken);
                await JS.InvokeVoidAsync("localStorage.setItem", "refreshToken", loginResponse.RefreshToken);

                var storedAccess = await JS.InvokeAsync<string>("localStorage.getItem", "accessToken");
                var storedRefresh = await JS.InvokeAsync<string>("localStorage.getItem", "refreshToken");
                await JS.InvokeVoidAsync("console.log", "[Login] LocalStorage токены", storedAccess, storedRefresh);

                Navigation.NavigateTo(string.IsNullOrWhiteSpace(ReturnUrl) ? "/admin/dashboard" : Uri.UnescapeDataString(ReturnUrl));
            }
            catch (Exception ex)
            {
                error = $"Ошибка {ex.Message}";
                await JS.InvokeVoidAsync("console.error", "[Login] Exception", ex.Message);
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }
}