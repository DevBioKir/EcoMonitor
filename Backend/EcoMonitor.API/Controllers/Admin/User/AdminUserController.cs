using System.Security.Claims;
using EcoMonitor.API.Attributes;
using EcoMonitor.App.Services.User;
using EcoMonitor.Contracts.Contracts.User;
using EcoMonitor.Contracts.Contracts.Users.UpdateUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoMonitor.API.Controllers.User;

[AdminApi]
[Authorize(Roles = "Admin")]
[Route("[controller]")]
public class AdminUserController(
    IUserService _userService,
    ILogger<AdminUserController> _logger)
    : BaseApiController<AdminUserController>(_logger)
{
    private Guid Actor()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            //return null;
            throw new UnauthorizedAccessException("User ID claim not found");
        }
        
        return Guid.Parse(userIdClaim.Value);
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAllUsersAsync(CancellationToken cancellationToken)
    {
        try
        {
            var actorId = Actor();
            var response = await _userService.GetAllAsync(actorId, cancellationToken);
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

    // [HttpPost("AddUser")]
    // public async Task<IActionResult> AddUserAsync(UserRequest user, CancellationToken cancellationToken)
    // {
    //     try
    //     {
    //         var actorId = Actor();
    //         await _userService.AddAsync(user, actorId, cancellationToken);
    //         return Ok();
    //     }
    //     catch (UnauthorizedAccessException ex)
    //     {
    //         _logger.LogWarning(ex, "Access denied in AddUser()");
    //         return Forbid();
    //     }
    //     catch (Exception e)
    //     {
    //         _logger.LogError(e, "Error adding user");
    //         return StatusCode(500, "Internal server error");
    //     }
    // }
    
    // TODO: Split the controller into use cases [HttpPatch] dictionary most likely
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUserAsync(string id, [FromBody] UpdateUserDTO request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(id, out var userId))
        {
            return BadRequest(new { error = "Invalid User ID format" });
        }
        _logger.LogInformation("📥 Контроллер получил JSON: {@Request}", request);
        
        //_logger.LogInformation("🔥 userId = {UserId}, request = {@Request}", userId, request);
    
        // if (!ModelState.IsValid)
        // {
        //     _logger.LogWarning("ModelState invalid для UserRequest: {@Errors}", ModelState);
        //     return BadRequest(ModelState);
        // }
    
        //_logger.LogInformation("Controller вызван, user.Id: {Id}", userId);
    
        // if (userId == Guid.Empty)
        //     return BadRequest(new { error = "User ID cannot be empty" });
        //
        // _logger.LogInformation("📧 Email перед сервисом: '{Email}'", request.Email);
        
        var currentUserId = Actor();
        await _userService.UpdateAsync(currentUserId, userId, request, cancellationToken);
        _logger.LogInformation("Контроллер ПОСЛЕ: {@Request}", request);
        return NoContent();
    }
    
    // [HttpPatch("{id}/personal-info")]
    // public async Task<IActionResult> UpdatePersonalInfoAsync(
    //     Guid id, UpdatePersonalInfoRequest updatePersonalInfoRequest, CancellationToken cancellationToken)
    // {
    //     var actorId = Actor();
    //     await _userService.UpdatePersonalInfoAsync(actorId, id, updatePersonalInfoRequest, cancellationToken);
    //     
    //     return Ok();
    // }
    //
    // [HttpPatch("{id}/email")]
    // public async Task<IActionResult> UpdateEmailAsync(
    //     Guid id, UpdateEmailRequest updateEmailRequest, CancellationToken cancellationToken)
    // {
    //     var actorId = Actor();
    //     await _userService.UpdateEmailAsync(actorId, id, updateEmailRequest, cancellationToken);
    //     
    //     return Ok();
    // }
    //
    // [HttpPatch("{id}/role")]
    // public async Task<IActionResult> UpdateUserRoleAsync(
    //     Guid userId, UpdateRoleRequest request, CancellationToken cancellationToken)
    // {
    //     var actorId = Actor();
    //     await _userService.UpdateUserRoleAsync(actorId, userId, request, cancellationToken);
    //     
    //     return Ok();
    // }
    
    // [HttpPut("UpdateUser")]
    // public async Task<IActionResult> UpdateUserAsync(UserRequest user, CancellationToken cancellationToke)
    // {
    //     if (!ModelState.IsValid)
    //     {
    //         _logger.LogWarning("ModelState invalid для UserRequest: {@Errors}", ModelState);
    //         return BadRequest(ModelState);
    //     }
    //
    //     _logger.LogInformation("Controller вызван, user.Id: {Id}", user.Id);
    //
    //     if (user.Id == Guid.Empty)
    //         return BadRequest(new { error = "User ID cannot be empty" });
    //     
    //     try
    //     {
    //         var currentUserId = CurrentUser();
    //         await _userService.UpdateAsync(user, currentUserId, cancellationToke);
    //         return Ok();
    //     }
    //     catch (UnauthorizedAccessException ex)
    //     {
    //         _logger.LogWarning(ex, "Access denied in UpdateUser()"); 
    //         return Forbid();
    //     }
    //     catch (Exception e)
    //     {
    //         Console.WriteLine(e);
    //         throw;
    //     }
    // }
}