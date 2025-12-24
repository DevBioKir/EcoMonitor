using EcoMonitor.AdminPanel.Data.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace EcoMonitor.AdminPanel.Components.Pages.admin;

public partial class UserManage : ComponentBase
{
    [Inject] private IHttpClientFactory HttpClientFactory { get; set; } = default!;
    private HttpClient AdminApi => HttpClientFactory.CreateClient("AdminApi");
        
    [Inject] private IJSRuntime JS { get; set; } = default!;

    private List<UserModelResponse> _users = new();
    private bool _loading = false;
    private string _errorMessage = "";
    
    private bool _createDialogOpen = false;
    private bool _creating = false;
    private CreateUserModelRequest _createModel = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadUsersAsync();
    }

    private async Task LoadUsersAsync()
    {
        try
        {
            _loading = true;
            Console.WriteLine("LoadUsersAsync: start");
            
            var result =
                await AdminApi.GetFromJsonAsync<List<UserModelResponse>>("/api/admin/v1/AdminUser/GetAll");
            _users = result ?? new();
        }
        catch (HttpRequestException ex)
        {
            _errorMessage = $"Ошибка сети: {ex.Message}";
            _users = new();
        }
        catch (Exception ex)
        {
            _errorMessage = $"Не удалось загрузить пользователей: {ex.Message}";
            _users = new();
        }
        finally
        {
            _loading = false;
        }
    }

    private void OpenCreateDialog()
    {
        _createModel = new CreateUserModelRequest();
        _createDialogOpen = true;
    }

    private void CloseCreateDialog()
    {
        _createDialogOpen = false;
    }

    private async Task CreateUserAsync()
    {
        _creating = true;

        try
        {
            var newUser = new CreateUserModelRequest
            {
                Firstname = _createModel.Firstname,
                Surname = _createModel.Surname,
                Email = _createModel.Email,
                Password = _createModel.Password
            };
            
            var response = await AdminApi.PostAsJsonAsync(
                "/api/admin/v1/AdminAuthorization/register", newUser);

            if (response.IsSuccessStatusCode)
            {
                _createDialogOpen = false;
                await LoadUsersAsync();
            }
            else
            {
                Console.WriteLine(response.Content.ReadAsStringAsync());
            }
        }
        catch (HttpRequestException ex)
        {
            _errorMessage = $"Ошибка сети: {ex.Message}";
        }
        catch (Exception ex)
        {
            _errorMessage = $"Не удалось создать пользователя: {ex.Message}";
        }
        finally
        {
            _creating = false;
        }
    }
    
    private async Task UpdateUserAsync()
    {
        _creating = true;

        try
        {
            var newUser = new CreateUserModelRequest
            {
                Firstname = _createModel.Firstname,
                Surname = _createModel.Surname,
                Email = _createModel.Email,
                Password = _createModel.Password
            };
            
            var response = await AdminApi.PostAsJsonAsync(
                "/api/admin/v1/AdminAuthorization/register", newUser);

            if (response.IsSuccessStatusCode)
            {
                _createDialogOpen = false;
                await LoadUsersAsync();
            }
            else
            {
                Console.WriteLine(response.Content.ReadAsStringAsync());
            }
        }
        catch (HttpRequestException ex)
        {
            _errorMessage = $"Ошибка сети: {ex.Message}";
        }
        catch (Exception ex)
        {
            _errorMessage = $"Не удалось создать пользователя: {ex.Message}";
        }
        finally
        {
            _creating = false;
        }
    }
}