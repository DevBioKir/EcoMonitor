using System.Security.Claims;
using EcoMonitor.App.Services;
using EcoMonitor.App.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoMonitor.API.Controllers.User;

[ApiController]
[Route("api/[controller]")]
public class UserController(
    IUserService _userService,
    ILogger<UserController> logger)
    : ControllerBase
{
    private readonly ILogger<UserController> _logger = logger;

    [HttpGet("Current user")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetCurrentUserAsync()
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var user = await _userService.GetByIdAsync(userId);
            
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user profile");
            return StatusCode(500, new  {message = ex.Message});
        }
    }
    
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsersAsync(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _userService.GetAllAsync(cancellationToken);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Error getting all users");
            return StatusCode(500, new {message = ex.Message});
        }
    }
    
    [HttpPost("Register")]
    [Authorize(Roles = "Admin")]
    
    
    
}