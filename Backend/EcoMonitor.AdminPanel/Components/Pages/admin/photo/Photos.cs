using System.Globalization;
using System.Net;
using EcoMonitor.AdminPanel.Data.Models;
using EcoMonitor.AdminPanel.Infrastucture.TokenPersistence;
using EcoMonitor.AdminPanel.Infrastucture.TokenStorage;
using EcoMonitor.Contracts.Contracts;
using EcoMonitor.Contracts.Contracts.BinPhoto;
using EcoMonitor.Contracts.Contracts.BinType;
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
    private string _uploadedBySearch = "";
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private ITokenPersistenceService TokenPersistence { get; set; } = default!;
    [Inject] private ITokenStore TokenStore { get; set; } = default!;
    [Inject] private ILogger<Photos> Logger { get; set; } = default!;
    
    private List<BinTypeResponse> _binTypes = new();
    
    private BinPhotoResponse? _selectedPhoto;
    private bool _detailsDialogOpen;
    
    private MudTable<BinPhotoResponse>? _table;
    
    private string SelectedPhotoUrl =>
        _selectedPhoto is null ? string.Empty : $"https://localhost:7198/{_selectedPhoto.UrlFile.Replace("\\","/")}";

    protected override async Task OnInitializedAsync()
    {
        Logger.LogInformation("Photos.OnInitializedAsync START");
        
        if (!TokenStore.HasAccessToken())
        {
            Navigation.NavigateTo("/auth/login", true); 
            return;
        }
        
        // bool tokensOk = await App.TokensLoaded.Task;
        //
        // if (!tokensOk || !TokenStore.HasAccessToken())
        // {
        //     Console.WriteLine("[UserManage] Tokens invalid or missing, redirecting to login");
        //     Navigation.NavigateTo("/auth/login", true);
        //     return;
        // }

        await LoadBinTypes();
        
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
        _uploadedBySearch = "";
        
        await LoadPhotosAsync();
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

    private void OpenEditDialog(BinPhotoResponse photo)
    {
        Console.WriteLine("OpenEditDialog CALLED");
        Console.WriteLine();
        _originalPhoto = photo;

        Console.WriteLine("Before: " + _editDialogOpen);

        _editModel = new EditPhotoModelRequest
        {
            Id = photo.Id,
            BinTypeId = photo.BinTypeId?.ToHashSet() ?? new HashSet<Guid>(),
            FillLevel = photo.FillLevel,
            IsOutsideBin = photo.IsOutsideBin,
            Comment = photo.Comment,
            TotalBins = photo.TotalBins
        };
        
        Console.WriteLine("=== PHOTO BIN TYPES ==="); 
        foreach (var id in photo.BinTypeId) 
            Console.WriteLine(id); 
        
        Console.WriteLine("=== LOADED BIN TYPES ===");
        foreach (var t in _binTypes) 
            Console.WriteLine($"{t.Id} => {t.Name}");

        Console.WriteLine();
        Console.WriteLine($"FillLevel: {_editModel.FillLevel}");
        Console.WriteLine($"IsOutsideBin: {_editModel.IsOutsideBin}");
        Console.WriteLine($"Comment: {_editModel.Comment}");
        Console.WriteLine($"TotalBins: {_editModel.TotalBins}");

        _newFiles.Clear();
        _previewImage = null;
        
        StateHasChanged();
        
        //await Task.Yield();
        _editDialogOpen = true;
        
        Console.WriteLine();
        Console.WriteLine("After: " + _editDialogOpen);
    }

    private void CloseEditDialog()
    {
        _editDialogOpen = false;
    }
    
    private void OpenDetailsDialog(BinPhotoResponse photo)
    {
        _selectedPhoto = photo;
        _detailsDialogOpen = true;
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
                    ? _dateRange.Start.Value.ToUniversalTime().Date 
                    : null,
                ToDate = _dateRange?.End.HasValue == true 
                    ? _dateRange.End.Value.ToUniversalTime().Date.AddDays(1).AddTicks(-1) 
                    : null,
                UploadedBySearch = _uploadedBySearch
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
                        p.Value is DateTime dt 
                            ? dt.ToUniversalTime().ToString("o") 
                            : p.Value.ToString())}"));
            //"o" - ISO 8601

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
        return string.Join(", ", 
            _binTypes
            .Where(t => binTypeIds.Contains(t.Id))
            .Select(t => t.Name));
    }

    private async Task OnPhotoSelected(IBrowserFile file)
    {
        _newFiles.Clear();
        _newFiles.Add(file);
        _editModel.Photo = file;
        
        using var stream = file.OpenReadStream(long.MaxValue); 
        using var ms = new MemoryStream(); 
        await stream.CopyToAsync(ms);
        
        _previewImage = $"data:{file.ContentType};base64,{Convert.ToBase64String(ms.ToArray())}";
        
        StateHasChanged();
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

            content.Add(new StringContent(_editModel.FillLevel.ToString(CultureInfo.InvariantCulture) ?? ""), "FillLevel");
            content.Add(new StringContent(_editModel.IsOutsideBin.ToString() ?? ""), "IsOutsideBin");
            content.Add(new StringContent(_editModel.Comment ?? ""), "Comment");
            content.Add(new StringContent(_editModel.TotalBins.ToString() ?? ""), "TotalBins");
            
            foreach (var id in _editModel.BinTypeId) 
                content.Add(new StringContent(id.ToString()), "BinTypeId");

            Console.WriteLine($"FileName {content.Headers.ContentDisposition?.FileName} {content.Headers.ContentDisposition?.FileName}");
            
            var response = await AdminApi.PutAsync(
                $"/api/admin/v1/AdminBinPhoto/{_editModel.Id}", 
                content);
            
            if (response.IsSuccessStatusCode)
            {
                _editDialogOpen = false;
                await LoadPhotosAsync();

                if (_selectedPhoto != null)
                {
                    _selectedPhoto = _photos.FirstOrDefault(p => p.Id == _selectedPhoto.Id);
                } 
                
                StateHasChanged();
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

    private async Task DeletePhoto(Guid id)
    {
        var response = await AdminApi.DeleteAsync($"/api/admin/v1/AdminBinPhoto/{id}");

        if (response.IsSuccessStatusCode)
        {
            await LoadPhotosAsync();
        }
        else
        {
            _errorMessage = await response.Content.ReadAsStringAsync();
        }
        // if (!response.IsSuccessStatusCode)
        // {
        //     throw new Exception("Не удалось удалить фото");
        // }
    }
    private void HandleRowClick(TableRowClickEventArgs<BinPhotoResponse> args)
    {
        Logger.LogInformation("🖱️ Row clicked: {Id} - {FileName}", 
            args.Item.Id, args.Item.FileName);
        
        _selectedPhoto = args.Item;
        _detailsDialogOpen = true;
        
        StateHasChanged();
    }
}