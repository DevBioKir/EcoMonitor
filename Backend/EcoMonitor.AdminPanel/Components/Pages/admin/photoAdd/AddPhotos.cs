using EcoMonitor.AdminPanel.Data.Models;
using EcoMonitor.Contracts.Contracts.BinType;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace EcoMonitor.AdminPanel.Components.Pages.admin.photoAdd;

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
    
    IList<IBrowserFile> _files = new List<IBrowserFile>();
    
    private List<BinTypeResponse> _binTypes = new();
    public string? _errorMessage { get; private set; }
    public string? PreviewImage { get; private set; }
    public bool IsLoading { get; private set; } = false;
    
    [Inject] private IHttpClientFactory HttpClientFactory { get; set; } = default!;
    private HttpClient PublicApi => HttpClientFactory.CreateClient("PublicApi");
    private HttpClient AdminApi => HttpClientFactory.CreateClient("AdminApi");

    [Inject] private IJSRuntime JS { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    protected override async void OnInitialized()
    {
        await LoadBinTypes();
        //Model = new AddPhotoRequest();
    }
    
    private async Task LoadBinTypes()
    {
        try
        {
            var response = await AdminApi.GetFromJsonAsync<List<BinTypeResponse>>(
                "/api/admin/v1/AdminBinType/GetAllBinTypes");
            _binTypes = response ?? new();
        }
        catch (Exception ex)
        {
            _errorMessage = $"Ошибка загрузки типов: {ex.Message}";
        }
    }

    private void AddFile(IBrowserFile file)
    {
        _files.Clear();
        _files.Add(file);
            
        if (file.Size > 10 * 1024 * 1024)
        { 
            _errorMessage = "Размер файла слишком велик. (>10Мб)"; 
            return;
        }
        
        Task.Run(async () =>
        {
            using var stream = file.OpenReadStream();
            using var memory = new MemoryStream();
            
            await stream.CopyToAsync(memory);
            var bytes = memory.ToArray();
        
            await InvokeAsync(() => 
            {
                PreviewImage = $"data:{file.ContentType};base64,{Convert.ToBase64String(bytes)}";
                StateHasChanged();
            });
        });
    }
    
    private void OnBinTypesChanged(IEnumerable<string> selectedValues)
    {
        Model.BinTypeCode = selectedValues.ToList();
        StateHasChanged();
    }

    private bool IsValid()
    {
        //return !string.IsNullOrEmpty(Model.Photo) &&
               return Model.BinTypeCode?.Any() == true &&
               Model.FillLevel >= 0 && Model.FillLevel <= 1 &&
               Model.TotalBins >= 1;
        // return !string.IsNullOrWhiteSpace(Model.Photo) &&
        //        (Model.BinTypeCode.Count != null) &&
        //        (Model.FillLevel != null) &&
        //        !string.IsNullOrWhiteSpace(Model.Comment) &&
        //        Model.TotalBins != null &&
        //        !string.IsNullOrWhiteSpace(PreviewImage);
    }

    public async Task HandleSubmit()
    {
        if (!IsValid())
        {
            _errorMessage = "Заполните все поля";
            return;
        }

        IsLoading = true;
        StateHasChanged();

        try
        {
            var file = _files.First();
            
            using var content = new MultipartFormDataContent();
            
            using var fileStream = file.OpenReadStream(10 * 1024 * 1024);
            using var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = 
                new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType ?? "image/jpeg");
            
            content.Add(fileContent, "Photo", file.Name);  // ✅ IFormFile ожидает "Photo"
            content.Add(new StringContent(Model.FillLevel.ToString("F2", 
                System.Globalization.CultureInfo.InvariantCulture)), "FillLevel");
            foreach (var binType in Model.BinTypeCode)
            {
                content.Add(new StringContent(binType), "BinTypeCode");
            }
            //content.Add(new StringContent(string.Join(",", Model.BinTypeCode)), "BinTypeCode");
            content.Add(new StringContent(Model.TotalBins.ToString()), "TotalBins");
            content.Add(new StringContent(Model.Comment ?? ""), "Comment");
            content.Add(new StringContent(Model.IsOutsideBin.ToString()), "IsOutsideBin");

            var response = await AdminApi.PostAsync(
                "/api/admin/v1/AdminBinPhoto/UploadWithMetadata", content);

            if (response.IsSuccessStatusCode)
            {
                Navigation.NavigateTo("/admin/photos");
            }
            else
            {
                _errorMessage = await response.Content.ReadAsStringAsync();
            }
        }
        catch (Exception ex)
        {
            _errorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }

    public void Cancel()
    {
        Navigation.NavigateTo("/admin/photo");
    }
}