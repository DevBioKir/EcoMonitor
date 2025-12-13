using System.Security.Claims;
using EcoMonitor.API.Attributes;
using EcoMonitor.App.Services.User;
using EcoMonitor.Contracts.Contracts.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoMonitor.API.Controllers.User;

[AdminApi]
[Authorize(Roles = "admin")]
[Route("[controller]")]
public class AdminUserController(
    IUserService _userService,
    ILogger<AdminUserController> _logger)
    : BaseApiController<AdminUserController>(_logger)
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

    [HttpGet]
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
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("AddUser")]
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
}