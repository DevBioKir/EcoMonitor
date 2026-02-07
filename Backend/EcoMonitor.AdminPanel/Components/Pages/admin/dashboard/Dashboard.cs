using EcoMonitor.Contracts.Contracts.BinPhoto;
using EcoMonitor.Core.Models.Users;
using Microsoft.AspNetCore.Components;

namespace EcoMonitor.AdminPanel.Components.Pages.admin.dashboard;

public partial class Dashboard
{
    [Inject] private IHttpClientFactory _clientFactory { get; set; }
    private HttpClient PublicApi => _clientFactory.CreateClient("PublicApi");
    private int? _countUsers;
    private bool _loading = false;
    private List<BinPhotoResponse> _latestPhotos = new();
    

    protected override async Task OnInitializedAsync()
    {
        _countUsers = await GetCountPhotosAsync();
        await LoadLatestPhotosAsync();
    }
    
    private async Task<int> GetCountPhotosAsync() => await PublicApi.GetFromJsonAsync<int>(
            "api/public/v1/Dashboard/CountPhotos");

    private async Task GetCountUsersAsync() => await PublicApi.GetFromJsonAsync<int>(
            "api/public/v1/Dashboard/CountUsers");

    private async Task LoadLatestPhotosAsync()
    {
        try
        {
            _loading = true;
            Console.WriteLine("Loading photos");

            var response = await PublicApi.GetFromJsonAsync<List<BinPhotoResponse>>("api/public/v1/Dashboard/LatestPhotos");
            
            _latestPhotos = response?.ToList() ?? new(); 
        }
        finally
        {
            _loading = false;
        }
    }
}