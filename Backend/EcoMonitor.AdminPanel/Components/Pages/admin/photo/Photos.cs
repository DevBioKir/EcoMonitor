using System.Net;
using EcoMonitor.AdminPanel.Data.Models;
using EcoMonitor.AdminPanel.Infrastucture.TokenPersistence;
using EcoMonitor.AdminPanel.Infrastucture.TokenStorage;
using EcoMonitor.Contracts.Contracts;
using EcoMonitor.Contracts.Contracts.BinPhoto;
using EcoMonitor.Core.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;

namespace EcoMonitor.AdminPanel.Components.Pages.admin.photo;

public partial class Photos : ComponentBase
{
    [Inject] private IHttpClientFactory HttpClientFactory { get; set; } = default!;
    private HttpClient AdminApi => HttpClientFactory.CreateClient("AdminApi");
    
    private HttpClient PublicApi => HttpClientFactory.CreateClient("PublicApi");

    [Inject] private IJSRuntime JS { get; set; } = default!;
    
    private List<BinPhotoResponse> _photos = new();
    private bool _loading = false;
    private string _errorMessage = "";
    
    private bool _editDialogOpen = false;
    private bool _editing = false;
    private BinPhotoResponse? _originalPhoto;
    
    private EditPhotoModelRequest _editModel = new();
    private List<IBrowserFile> _newFiles = new();
    private string? _previewImage;
    
    private string _search = string.Empty;
    
    // PAGING
    private int _page = 1;
    private int _pageSize = 20;
    private int _totalCount;
    
    // FILTERS
    private DateRange? _dateRange;
    private double[] _fillRange = [0, 1];
    private bool? _outsideFilter  = null;
    private string _sortBy = "dateDesc";
    
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private ITokenPersistenceService TokenPersistence { get; set; } = default!;
    [Inject] private ITokenStore TokenStore { get; set; } = default!;
    [Inject] private ILogger<Photos> Logger { get; set; } = default!;
    
    private Dictionary<Guid, string> _binTypesName = new ()
    {
        [BinTypeConstants.Paper] = "Бумага и картон",
        [BinTypeConstants.Universal] = "Смешанные отходы",
        [BinTypeConstants.Glass] = "Стекло",
        [BinTypeConstants.Organic] = "Органика и пищевые отходы",
        [BinTypeConstants.Plastic] = "Пластик",
        [BinTypeConstants.Metal] = "Металл"
    };

    protected override async Task OnInitializedAsync()
    {
        Logger.LogInformation("Photos.OnInitializedAsync START");
        
        bool tokensOk = await App.TokensLoaded.Task;

        if (!tokensOk || !TokenStore.HasAccessToken())
        {
            Console.WriteLine("[UserManage] Tokens invalid or missing, redirecting to login");
            Navigation.NavigateTo("/auth/login", true);
            return;
        }
        
        await LoadPhotosAsync();
    }

    private async Task ApplyFilters()
    {
        _page = 1;
        await LoadPhotosAsync();
    }

    private async Task ResetFilters()
    {
        _page = 1;
        _dateRange = null;
        _fillRange = [0, 1];
        _outsideFilter = null;
        _sortBy = "dateDesc";
        
        await LoadPhotosAsync();
    }

    private void OpenEditDialog(BinPhotoResponse photo)
    {
        _originalPhoto = photo;

        _editModel = new EditPhotoModelRequest
        {
            Id = photo.Id,
            BinTypeId = photo.BinTypeId.ToList(),
            FillLevel = photo.FillLevel,
            IsOutsideBin = photo.IsOutsideBin,
            Comment = photo.Comment,
            TotalBins = photo.TotalBins
        };
        _newFiles.Clear();
        _previewImage = null;
        
        _editDialogOpen = true;
    }

    private void CloseEditDialog()
    {
        _editDialogOpen = false;
    }

    private async Task LoadPhotosAsync()
    {
        try
        {
            _loading = true;
            Console.WriteLine("Loading photos");

            var filterDate = new PhotoFilterDTO()
            {
                Page = _page,
                PageSize = _pageSize,
                SortBy = _sortBy,
                OnlyOutsideBin = _outsideFilter,
                MinFillLevel = _fillRange[0],
                MaxFillLevel = _fillRange[1],
                FromDate = _dateRange?.Start.HasValue == true 
                    ? DateTime.SpecifyKind(_dateRange.Start.Value.Date, DateTimeKind.Utc)
                    : null,
                ToDate = _dateRange?.End.HasValue == true 
                    ? DateTime.SpecifyKind(_dateRange.End.Value.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc)
                    : null
            };
            
            Console.WriteLine("=== FILTER DTO ===");
            Console.WriteLine($"Page: {filterDate.Page}"); 
            Console.WriteLine($"PageSize: {filterDate.PageSize}");
            Console.WriteLine($"SortBy: {filterDate.SortBy}"); 
            Console.WriteLine($"OnlyOutsideBin: {filterDate.OnlyOutsideBin}"); 
            Console.WriteLine($"MinFillLevel: {filterDate.MinFillLevel}"); 
            Console.WriteLine($"MaxFillLevel: {filterDate.MaxFillLevel}"); 
            Console.WriteLine($"FromDate: {filterDate.FromDate}"); 
            Console.WriteLine($"ToDate: {filterDate.ToDate}"); 

            var query = string.Join('&',
                filterDate.GetType().GetProperties().Select(p => new
                    {
                        p.Name,
                        Value = p.GetValue(filterDate)
                    })
                    .Where(p => p.Value is not null)
                    .Select(p => $"{p.Name}={Uri.EscapeDataString(
                        p.Value is DateTime dt ? dt.ToString("yyyy-MM-dd") : p.Value.ToString())}"));

            var url = $"/api/admin/v1/AdminBinPhoto/allPhotos?{query}";
            
            Console.WriteLine("=== QUERY STRING ==="); 
            Console.WriteLine(query);
            Console.WriteLine("=== FINAL URL ==="); 
            Console.WriteLine(url);
            
            //var result = await AdminApi.GetFromJsonAsync<PagedResultDTO<BinPhotoResponse>>(url);
            var response = await AdminApi.GetAsync(url);
            
            if (response.StatusCode == HttpStatusCode.Unauthorized) { 
                // AdminAuthHandler уже вызвал TriggerRedirect()
                return;
            }

            if (!response.IsSuccessStatusCode)
            {
                _errorMessage = "Ошибка загрузки данных"; 
                return;
            } 
            
            var result = await response.Content.ReadFromJsonAsync<PagedResultDTO<BinPhotoResponse>>(); 
            
            _photos = result?.Items?.ToList() ?? new(); 
            _totalCount = result?.TotalCount ?? 0;
        }
        // catch (Exception ex)
        // {
        //     _errorMessage = $"Не удалось загрузить фотографии: {ex.Message}";
        //     _photos = new();
        // }
        finally
        {
            _loading = false;
        }
    }
    
    private string GetBinTypeName(List<Guid> binTypeIds)
    {
        return string.Join(", ", binTypeIds
            .Where(id => _binTypesName.ContainsKey(id))
            .Select(id => _binTypesName[id]));
    }

    private async Task OnPhotoSelected(IBrowserFile file)
    {
        _newFiles.Clear();
        _newFiles.Add(file);
        
        _editModel.Photo = file;
        
        using var stream = file.OpenReadStream(10 * 1024 * 1024); 
        using var ms = new MemoryStream(); 
        await stream.CopyToAsync(ms);
        
        _previewImage = $"data:{file.ContentType};base64,{Convert.ToBase64String(ms.ToArray())}";
    }

    private async Task UpdatePhotoAsync()
    {
        _editing = true;

        try
        {
            var content = new MultipartFormDataContent();
            if (_editModel.Photo is not null)
            {
                var stream = _editModel.Photo.OpenReadStream(long.MaxValue);
                var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType =
                    new System.Net.Http.Headers.MediaTypeHeaderValue(_editModel.Photo.ContentType);
                
                content.Add(fileContent, "Photo", _editModel.Photo.Name);
            }

            content.Add(new StringContent(_editModel.FillLevel?.ToString() ?? ""), "FillLevel");
            content.Add(new StringContent(_editModel.IsOutsideBin?.ToString() ?? ""), "IsOutsideBin");
            content.Add(new StringContent(_editModel.Comment ?? ""), "Comment");
            content.Add(new StringContent(_editModel.TotalBins?.ToString() ?? ""), "TotalBins");
            
            foreach (var id in _editModel.BinTypeId) 
                content.Add(new StringContent(id.ToString()), "BinTypeId");
            
            var response = await AdminApi.PutAsync(
                $"/api/admin/v1/AdminBinPhoto/{_editModel.Id}", 
                content);
            
            if (response.IsSuccessStatusCode)
            {
                _editDialogOpen = false;
                await LoadPhotosAsync();
            }
            else
            {
                _errorMessage = await response.Content.ReadAsStringAsync();
            }
        }
        finally
        {
            _editing = false;
        }

    }
}