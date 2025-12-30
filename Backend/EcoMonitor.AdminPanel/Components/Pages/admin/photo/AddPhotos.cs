using EcoMonitor.AdminPanel.Data.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace EcoMonitor.AdminPanel.Components.Pages.admin.photo;

public partial class AddPhotos : ComponentBase
{
    public AddPhotoRequest Model { get; set; } = new()
    {
        BinTypeCode = new List<string>(),
        FillLevel = 0,
        IsOutsideBin = false,
        Comment = string.Empty,
        TotalBins = 1
    };
    public string? _errorMessage { get; private set; }
    public string? PreviewImage { get; private set; }
    public bool IsLoading { get; private set; } = false;
    
    [Inject] private IHttpClientFactory HttpClientFactory { get; set; } = default!;
    private HttpClient PublicApi => HttpClientFactory.CreateClient("PublicApi");

    [Inject] private IJSRuntime JS { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    protected override void OnInitialized()
    {
        Model = new AddPhotoRequest();
        
    }

    private async Task AddFile(IBrowserFile[] files)
    {
        if (files.Length > 0)
        {
            var file = files[0];
            if (file.Size > 10 * 1024 * 1024)
            {
                _errorMessage = "Размер файла слишком велик. (>10Мб)";
                return;
            } 
            
            var buffer = new byte[file.Size];
            await file.OpenReadStream().ReadExactlyAsync(buffer);
            PreviewImage = $"data:{file.ContentType};base64,{Convert.ToBase64String(buffer)}";
            
            Model.Photo = Convert.ToBase64String(buffer);
            
            StateHasChanged();
        }
    }

    private bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Model.Photo) &&
               (Model.BinTypeCode.Count != null) &&
               (Model.FillLevel != null) &&
               !string.IsNullOrWhiteSpace(Model.Comment) &&
               Model.TotalBins != null &&
               !string.IsNullOrWhiteSpace(PreviewImage);
    }

    public async Task HandleSubmit()
    {
        try
        {
            IsLoading = true;
            _errorMessage = null;

            var response = await PublicApi.PostAsJsonAsync(
                "/api/public/v1/BinPhoto/UploadWithMetadata", Model);

            if (response.IsSuccessStatusCode)
            {
                IsLoading = false;
                OnInitialized();
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
            IsLoading = false;
        }
    }

    public void Cancel()
    {
        Navigation.NavigateTo("/admin/photos");
    }
}