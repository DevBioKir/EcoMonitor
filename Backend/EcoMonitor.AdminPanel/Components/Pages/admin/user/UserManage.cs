using EcoMonitor.AdminPanel.Data.Models;
using EcoMonitor.AdminPanel.Infrastucture.TokenPersistence;
using EcoMonitor.AdminPanel.Infrastucture.TokenStorage;
using EcoMonitor.Contracts.Contracts.BlockUser;
using EcoMonitor.Contracts.Contracts.User;
using EcoMonitor.Contracts.Contracts.Users.UpdateUser;
using EcoMonitor.Core.Models.Users;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace EcoMonitor.AdminPanel.Components.Pages.admin.user;

public partial class UserManage : ComponentBase
{
    [Inject] private IHttpClientFactory HttpClientFactory { get; set; } = default!;
    private HttpClient AdminApi => HttpClientFactory.CreateClient("AdminApi");
        
    [Inject] private IJSRuntime JS { get; set; } = default!;
    [Inject] private ITokenStore TokenStore { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private ITokenPersistenceService TokenPersistence { get; set; } = default!;

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
    
    private bool _blockDialogOpen = false;
    private bool _blocking = false;
    private BlockUserUIModel _blockModel = new();
    private UserModelResponse _userToBlock;
    
    private string _search = string.Empty;
        
    private Guid _currentRoleId = Guid.Empty;
    private bool _initialized;
    
    [Inject] private ILogger<UserManage> Logger { get; set; } = default!;
    
    // protected override async Task OnAfterRenderAsync(bool firstRender)
    // {
    //     if (!firstRender || !_initialized) return;
    //     
    //     var storedAccess = await JS.InvokeAsync<string>("localStorage.getItem", "accessToken");
    //     var storedRefresh = await JS.InvokeAsync<string>("localStorage.getItem", "refreshToken");
    //     await TokenPersistence.LoadAsync();
    //     TokenStore.SetTokens(storedAccess, storedRefresh);
    //     
    //     Console.WriteLine("[UserManage] Проверка токенов после загрузки из localStorage");
    //     Console.WriteLine($"HasAccessToken: {TokenStore.HasAccessToken()}");
    //     Console.WriteLine($"AccessToken: {TokenStore.AccessToken}");
    //     Console.WriteLine($"RefreshToken: {TokenStore.RefreshToken}");
    //
    //     await LoadUsersAsync();
    //     _initialized = true;
    //     // if (!firstRender) return;
    //     //
    //     // // Получаем токены из localStorage
    //     // // var storedAccess = await JS.InvokeAsync<string>("localStorage.getItem", "accessToken");
    //     // // var storedRefresh = await JS.InvokeAsync<string>("localStorage.getItem", "refreshToken");
    //     // //await TokenPersistence.LoadAsync();
    //     //
    //     // // TokenStore.SetTokens(storedAccess, storedRefresh);
    //     //
    //     // Console.WriteLine("[UserManage] Проверка токенов после загрузки из localStorage");
    //     // Console.WriteLine($"HasAccessToken: {TokenStore.HasAccessToken()}");
    //     // Console.WriteLine($"AccessToken: {TokenStore.AccessToken}");
    //     // Console.WriteLine($"RefreshToken: {TokenStore.RefreshToken}");
    //     //
    //     // if (!TokenStore.HasAccessToken())
    //     // {
    //     //     Navigation.NavigateTo("/auth/login", true);
    //     //     return;
    //     // }
    //     //
    //     // await LoadUsersAsync();
    //     // _initialized = true;
    //     // StateHasChanged();
    // }

    protected override async Task OnInitializedAsync()
    {
        Logger.LogInformation("UserManager.OnInitializedAsync START");

        if (!TokenStore.HasAccessToken())
        {
            Navigation.NavigateTo("/auth/login", true); 
            return;
        }
        
        // if (_initialized) return; // защита от повторного вызова
        // _initialized = true;
        
        // Console.WriteLine("[UserManage] Waiting for tokens from Index...");
        // bool tokensOk = await App.TokensLoaded.Task;
        //
        // if (!tokensOk || !TokenStore.HasAccessToken())
        // {
        //     Console.WriteLine("[UserManage] Tokens invalid or missing, redirecting to login");
        //     Navigation.NavigateTo("/auth/login", true);
        //     return;
        // }

        Console.WriteLine("[UserManage] Tokens received successfully!");
        Console.WriteLine($"[UserManage] AccessToken: {TokenStore.AccessToken}");
        Console.WriteLine($"[UserManage] RefreshToken: {TokenStore.RefreshToken}");
        
        await LoadUsersAsync();

        _ = Task.Run(async () =>
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromMinutes(5));
                await InvokeAsync(LoadUsersAsync);
            }
        });

        // Console.WriteLine("[AllUsers] uploaded by users");
        // foreach (var user in _users)
        // {
        //     Console.WriteLine($"User id: {user.Id}");
        //     Console.WriteLine($"User Firstname: {user.Firstname}");
        //     Console.WriteLine($"User Surname: {user.Surname}");
        //     Console.WriteLine($"User Email: {user.Email}");
        //     Console.WriteLine($"User RoleUser: {user.RoleUser}");
        //     Console.WriteLine($"User AccountEnabled: {user.AccountEnabled}");
        //     Console.WriteLine($"User BlockReason: {user.BlockReason}");
        //     Console.WriteLine($"User LockedUntil: {user.LockedUntil}");
        // }


        // var accessToken = await JS.InvokeAsync<string>("localStorage.getItem", "accessToken");
        // var refreshToken = await JS.InvokeAsync<string>("localStorage.getItem", "refreshToken");
        // TokenStore.SetTokens(accessToken, refreshToken);
        //
        // Console.WriteLine($"[UserManage] Проверка токенов после загрузки из localStorage");
        // Console.WriteLine($"[UserManage] HasAccessToken: {TokenStore.HasAccessToken()}");
        // Console.WriteLine($"[UserManage] AccessToken: {TokenStore.AccessToken}");
        // Console.WriteLine($"[UserManage] RefreshToken: {TokenStore.RefreshToken}");
        //
        // if (!TokenStore.HasAccessToken())
        // {
        //     Navigation.NavigateTo("/auth/login");
        //     return;
        // }
        //
        // await LoadUsersAsync();
    }

    private async Task LoadUsersAsync()
    {
        try
        {
            _loading = true;
            //_search = "";
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
            await InvokeAsync(StateHasChanged);
        }
    }

    private void OpenCreateDialog()
    {
        _createModel = new CreateUserModelRequest
        {
            RoleName = RoleConstants.UserId
        };
        
        // _createModel.RoleNameId = _createModel.RoleNameId switch
        // {
        //     var id when id == UserRoleUser.Id => UserRoleUser,
        //     var id when id == UserRoleManager.Id => UserRoleManager,
        //     var id when id == UserRoleAdmin.Id => UserRoleAdmin,
        //     _ => _createModel.RoleNameId
        // };
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
            RoleId = user.RoleUser.Id,
            Role = user.RoleUser
        };
        
        _editModel.Role = _editModel.RoleId switch
        {
            var id when id == UserRoleUser.Id => UserRoleUser,
            var id when id == UserRoleManager.Id => UserRoleManager,
            var id when id == UserRoleAdmin.Id => UserRoleAdmin,
            _ => _editModel.Role
        };
        
        _editDialogOpen = true;
        
        Console.WriteLine("Это данные пользователя которые будут подвержены изменению");
        Console.WriteLine($"CurrentUser: {System.Text.Json.JsonSerializer.Serialize(_editModel)}");
    }

    private void CloseEditDialog()
    {
        _editDialogOpen = false;
    }
    
    private void OpenBlockDialog(UserModelResponse user)
    {
        _userToBlock = user;
        _blockModel = new BlockUserUIModel()
        {
            Days = 1,
            Reason = "Увольнение"
        };
        _blockDialogOpen = true;
        Console.WriteLine($"NewUser: {System.Text.Json.JsonSerializer.Serialize(_userToBlock)}");
        
    }

    private void CloseBlockDialog()
    {
        _blockDialogOpen = false;
        _userToBlock =  null;
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
            Console.WriteLine($"DEBUG: Создаём пользователя: {_createModel.Firstname}, {_createModel.Surname}, Role: '{_createModel.RoleName}'");
            
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
    
    // private void OnRoleChanged(UserRoleResponse role)
    // {
    //     _editModel.Role = role;
    //     _editModel.RoleId = role.Id;
    // }
    
    private async Task UpdateUserAsync()
    {
        _editing = true;
        try
        {
            Console.WriteLine($"DEBUG: RoleId {_editModel.RoleId}");
            Console.WriteLine($"DEBUG Role: {System.Text.Json.JsonSerializer.Serialize(_editModel.Role)}");
            
            Console.WriteLine($"🔥 Id: '{_editModel.Id}'");
            Console.WriteLine($"🔥 Firstname: '{_editModel.Firstname}'");
            Console.WriteLine($"🔥 Surname: '{_editModel.Surname}'"); 
            Console.WriteLine($"🔥 Email: '{_editModel.Email}'");
            Console.WriteLine($"🔥 RoleId: '{_editModel.RoleId}'");
            
            var userData = new UpdateUserDTO
            {
                Firstname = _editModel.Firstname ?? "",
                Surname = _editModel.Surname ?? "",
                Email = _editModel.Email ?? "",
                UserRoleId = _editModel.Role.Id
            };
            
            Console.WriteLine($"NewUser: {System.Text.Json.JsonSerializer.Serialize(userData)}");

            var response = await AdminApi.PutAsJsonAsync(
                $"/api/admin/v1/AdminUser/{_editModel.Id}", userData);

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
        }
    }

    private async Task BlockUserAsync()
    {
        if (_userToBlock == null) 
            return;
        Console.WriteLine($"NewUserId: {_userToBlock.Id}");
        _blocking = true;
    
        try
        {
            var request = new BlockUserRequest(
                _blockModel.Days,
                _blockModel.Reason
            );
            
            Console.WriteLine($"NewUserId: {_userToBlock.Id}");
            
            var response = await AdminApi.PostAsJsonAsync(
                $"api/admin/v1/AdminAuthorization/block/{_userToBlock.Id}", request);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Пользователь {_userToBlock.Email} заблокирован успешно");

                if (response.IsSuccessStatusCode)
                {
                    CloseBlockDialog();
                    await LoadUsersAsync();
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Ошибка блокировки: {errorContent}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка блокировки: {ex.Message}");
        }
        finally
        {
            _blocking = false;
        }
    }
    
    private static readonly UserRoleResponse UserRoleUser = new(
        RoleConstants.UserId, "User", "Обычный пользователь", new List<PermissionResponse>());

    private static readonly UserRoleResponse UserRoleManager = new(
        RoleConstants.ManagerId, "Manager", "Менеджер", new List<PermissionResponse>());
    
    private static readonly UserRoleResponse UserRoleAdmin = new(
        RoleConstants.AdminId, "Admin", "Администратор", new List<PermissionResponse>());


    private static bool IsBlocked(UserModelResponse user)
    {
        //Console.WriteLine($"IsBlocked User: {user.LockedUntil.HasValue}, {user.LockedUntil}");
        return user.LockedUntil.HasValue &&
               user.LockedUntil > DateTime.UtcNow;
    }

    private static string GetBlockInfo(UserModelResponse user)
    {
        if (!IsBlocked(user))
            return string.Empty;
        
        return $"Причина: {user.BlockReason}, до {user.LockedUntil.Value.ToLocalTime():dd.MM.yyyy HH:mm}";
    }

    private async Task OpenUnlock(UserModelResponse user)
    {
        _userToBlock = user;
        await UnlockUser();
    }

    private async Task UnlockUser()
    {
        if (_userToBlock == null) 
            return;
        
        Console.WriteLine($"User to unlock: {_userToBlock.Id}");
        _blocking = true;
    
        try
        {
            Console.WriteLine($"NewUserId: {_userToBlock.Id}");
            
            var response = await AdminApi.PostAsync(
                $"api/admin/v1/AdminAuthorization/unlock/{_userToBlock.Id}", 
                content: null);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Пользователь {_userToBlock.Email} разблокирован успешно");
                await LoadUsersAsync();

                // if (response.IsSuccessStatusCode)
                // {
                //     CloseBlockDialog();
                //     //await LoadUsersAsync(); 
                // }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Ошибка разблокировки: {errorContent}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка разблокировки: {ex.Message}");
        }
        finally
        {
            _blocking = false;
        }
    }

    private bool FilterUser(UserModelResponse user)
    {
        if (string.IsNullOrWhiteSpace(_search))
            return true;
        
        return 
            user.Firstname.Contains(_search,  StringComparison.OrdinalIgnoreCase) ||
            user.Surname.Contains(_search,  StringComparison.OrdinalIgnoreCase) ||
            user.Email.Contains(_search,  StringComparison.OrdinalIgnoreCase);
    }
    
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