using EcoMonitor.Contracts.Contracts.BinPhoto;
using EcoMonitor.Core.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace EcoMonitor.AdminPanel.Components.Pages.admin.photo;

public partial class Photos : ComponentBase
{
    [Inject] private IHttpClientFactory HttpClientFactory { get; set; } = default!;
    private HttpClient AdminApi => HttpClientFactory.CreateClient("AdminApi");

    [Inject] private IJSRuntime JS { get; set; } = default!;
    
    private List<BinPhotoResponse> _photos = new();
    private bool _loading = false;
    private string _errorMessage = "";
    
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
        await LoadPhotosAsync();
    }

    private async Task LoadPhotosAsync()
    {
        try
        {
            _loading = true;
            Console.WriteLine("Loading photos");
            
            var result = await AdminApi.GetFromJsonAsync<List<BinPhotoResponse>>(
                "/api/admin/v1/AdminBinPhoto/GetAllPhotos");
            _photos.AddRange(result);
        }
        catch (HttpRequestException ex)
        {
            _errorMessage = $"Ошибка сети: {ex.Message}";
            _photos = new();
        }
        catch (Exception ex)
        {
            _errorMessage = $"Не удалось загрузить фотографии: {ex.Message}";
            _photos = new();
        }
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
}