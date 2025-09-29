using System.Security.Claims;
using EcoMonitor.App.Services;
using EcoMonitor.App.Services.User;
using EcoMonitor.Contracts.Contracts.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoMonitor.API.Controllers.User;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController(
    IUserService _userService,
    ILogger<UserController> logger)
    : ControllerBase
{
    private readonly ILogger<UserController> _logger = logger;

    private Guid CurrentUser() => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
    
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsersAsync(CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = CurrentUser();
            var response = await _userService.GetAllAsync(currentUserId, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Access denied in GetAll()"); 
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Error getting all users");
            return StatusCode(500, new {message = ex.Message});
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUserAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userService.GetByIdAsync(id, cancellationToken);
            if (user == null) return NotFound();
            return Ok(user);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting user");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("ByEmail")]
    public async Task<IActionResult> GetUserByEmailAsync([FromQuery] string email, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userService.GetByEmailAsync(email, cancellationToken);
            if (user == null) return NotFound();
            return Ok(user);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting user by email");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("AddUser")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddUserAsync(UserRequest user, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = CurrentUser();
            await _userService.AddAsync(user, currentUserId, cancellationToken);
            return Ok();
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Access denied in AddUser()"); 
            return Forbid();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error adding user");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("UpdateUser")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateUserAsync(UserRequest user, CancellationToken cancellationToke)
    {
        try
        {
            var currentUserId = CurrentUser();
            await _userService.UpdateAsync(user, currentUserId, cancellationToke);
            return Ok();
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Access denied in UpdateUser()"); 
            return Forbid();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUserAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = CurrentUser();
            await _userService.DeleteAsync(id, currentUserId, cancellationToken);
            return Ok();
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Access denied in DeleteUser()"); 
            return Forbid();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    
    
}