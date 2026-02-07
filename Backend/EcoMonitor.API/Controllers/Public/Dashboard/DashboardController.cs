using EcoMonitor.API.Attributes;
using EcoMonitor.App.Services;
using EcoMonitor.App.Services.User;
using EcoMonitor.Contracts.Contracts.BinPhoto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoMonitor.API.Controllers.Dashboard;

[PublicApi]
[Authorize]
[Route("[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IBinPhotoService  _photoService;
    
    public DashboardController(IUserService userService, IBinPhotoService photoService)
    {
        _userService = userService;
        _photoService = photoService;
    }
    
    [AllowAnonymous]
    [HttpGet("CountUsers")]
    public async Task<ActionResult<int>> GetCountUsersAsync(CancellationToken cancellationToken = default)
    {
        var count = await _userService.GetCountUsersAsync(cancellationToken); 
        return Ok(count);
    }
    
    [AllowAnonymous]
    [HttpGet("CountPhotos")]
    public async Task<ActionResult<int>> GetCountPhotosAsync()
    {
        return await _photoService.GetCountPhotosAsync();
    }
    
    [AllowAnonymous]
    [HttpGet("LatestPhotos")]
    public async Task<ActionResult<IReadOnlyList<BinPhotoResponse>>> GetLatestPhotosAsync(
        int count = 5,
        CancellationToken cancellationToken = default)
    {
        var response = await _photoService.GetLatestPhotosAsync(count, cancellationToken);
        return Ok(response);
    }
    
}