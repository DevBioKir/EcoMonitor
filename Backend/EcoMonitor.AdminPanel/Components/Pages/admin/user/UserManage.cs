using EcoMonitor.AdminPanel.Data.Models;
using EcoMonitor.Contracts.Contracts.User;
using EcoMonitor.Contracts.Contracts.Users.UpdateUser;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace EcoMonitor.AdminPanel.Components.Pages.admin.user;

public partial class UserManage : ComponentBase
{
    [Inject] private IHttpClientFactory HttpClientFactory { get; set; } = default!;
    private HttpClient AdminApi => HttpClientFactory.CreateClient("AdminApi");
        
    [Inject] private IJSRuntime JS { get; set; } = default!;

    private List<UserModelResponse> _users = new();
    private UserModelResponse _originalUser;
    private bool _loading = false;
    private string _errorMessage = "";
    
    private bool _createDialogOpen = false;
    private bool _creating = false;
    private CreateUserModelRequest _createModel = new();
    
    private bool _editDialogOpen = false;
    private bool _editing = false;
    private EditUserModelRequest _editModel = new();

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

    private void OpenEditDialog(UserModelResponse user)
    {
        Console.WriteLine($"Происходит изменение: {user.Id}");
        
        _originalUser = user;
        
        _editModel = new EditUserModelRequest
        {
            Id = user.Id,
            Firstname = user.Firstname,
            Surname = user.Surname,
            Email = user.Email,
            Role = user.RoleUser
        };
        _editDialogOpen = true;
    }

    private void CloseEditDialog()
    {
        _editDialogOpen = false;
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
                Password = _createModel.Password,
                RoleName = _createModel.RoleName,
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
        _editing = true;
        StateHasChanged();

        try
        {
            var newUser = new UpdateUserDTO(
                _editModel.Firstname, 
                _editModel.Surname, 
                _editModel.Email, 
                _editModel.Role.Id);
            
            Console.WriteLine($"NewUser: {System.Text.Json.JsonSerializer.Serialize(newUser)}");

            var response = await AdminApi.PutAsJsonAsync(
                $"/api/admin/v1/AdminUser/{_editModel.Id}", newUser);

            if (response.IsSuccessStatusCode)
            {
                _editDialogOpen = false;
                await LoadUsersAsync();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Ошибка: {errorContent}");
                _errorMessage = $"Ошибка обновления: {errorContent}";
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
            _editing = false;
            StateHasChanged();
        }
    }

    // private async Task BlockUserAsync()
    // {
    //     _creating = true;
    //
    //     try
    //     {
    //         await AdminApi.PostAsync($"api/admin/v1/AdminAuthorization/block{id}", _createModel);
    //     }
    //     catch (Exception e)
    //     {
    //         Console.WriteLine(e);
    //         throw;
    //     }
    // }
    
    private static readonly UserRoleResponse UserRoleUser = new(
        Guid.Empty, "User", "Обычный пользователь", new List<PermissionResponse>());

    private static readonly UserRoleResponse UserRoleManager = new(
        Guid.Empty, "Manager", "Менеджер", new List<PermissionResponse>());
    
    private static readonly UserRoleResponse UserRoleAdmin = new(
        Guid.Empty, "Admin", "Администратор", new List<PermissionResponse>());


    // private bool HasPersonalInfoChanged()
    // {
    //     return _originalUser != null && 
    //            (_originalUser.Firstname != _editModel.Firstname ||
    //             _originalUser.Surname != _editModel.Surname);
    // }
    //
    // private bool HasEmailChanged()
    // {
    //     return _originalUser != null && 
    //            _originalUser.Email != _editModel.Email;
    // }
    //
    // private bool HasRoleChanged()
    // {
    //     return _originalUser != null && 
    //            _originalUser.RoleUser != _editModel.Role;
    // }
}