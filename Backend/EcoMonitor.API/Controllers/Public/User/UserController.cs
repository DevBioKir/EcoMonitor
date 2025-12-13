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
    ILogger<UserController> _logger)
    : ControllerBase
{
    private Guid? CurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return null;
            //throw new UnauthorizedAccessException("User ID claim not found");
        }
        return Guid.Parse(userIdClaim.Value);
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserResponse>> GetCurrentUser()
    {
        var userId = CurrentUser();
        if (userId == null) 
        {
            return Unauthorized("User is not authenticated");
        }
        return await _userService.GetByIdAsync(userId.Value, CancellationToken.None);
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

    // [HttpDelete("{id:guid}")]
    // //[Authorize(Roles = "Admin")]
    // public async Task<IActionResult> DeleteUserAsync(Guid id, CancellationToken cancellationToken)
    // {
    //     try
    //     {
    //         var currentUserId = CurrentUser();
    //         await _userService.DeleteAsync(id, currentUserId, cancellationToken);
    //         return Ok();
    //     }
    //     catch (UnauthorizedAccessException ex)
    //     {
    //         _logger.LogWarning(ex, "Access denied in DeleteUser()"); 
    //         return Forbid();
    //     }
    //     catch (Exception e)
    //     {
    //         Console.WriteLine(e);
    //         throw;
    //     }
    // }
    
    // [HttpDelete("Delete")]
    // public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    // {
    //     try
    //     {
    //         await _userService.DeleteUserAsync(id, cancellationToken);
    //         return Ok();
    //     }
    //     catch (UnauthorizedAccessException ex)
    //     {
    //         _logger.LogWarning(ex, "Access denied in DeleteUser()"); 
    //         return Forbid();
    //     }
    //     catch (Exception e)
    //     {
    //         Console.WriteLine(e);
    //         throw;
    //     }
    // }
}