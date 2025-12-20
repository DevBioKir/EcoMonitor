using EcoMonitor.API.Attributes;
using EcoMonitor.App.Services;
using EcoMonitor.Contracts.Contracts.Auth;
using EcoMonitor.Contracts.Contracts.BlockUser;
using EcoMonitor.Contracts.Contracts.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoMonitor.API.Controllers.Authorization;

[AdminApi]
[Authorize(Roles = "Admin")]
[Route("[controller]")]
public class AdminAuthorizationController(
    ILogger<AdminAuthorizationController> logger,
    IAuthService authService)
    : BaseApiController<AdminAuthorizationController>(logger)
{
    [AllowAnonymous]
    [HttpPost("admin-login")]
    public async Task<IActionResult> AdminLoginAsync([FromBody] AuthRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Password) && string.IsNullOrWhiteSpace(request.Email))
        {
            _logger.LogWarning("login and password must not be empty");
            return BadRequest(new { message = "Current and new passwords are required" });
        }
        
        try
        {
            var response = await authService.LoginAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Failed to login attempt for user: {Email}", request.Email);
            
            if (ex.Message.Contains("пользователь не найден", StringComparison.OrdinalIgnoreCase))
            { 
                return NotFound(new 
                { 
                    message = "Пользователь не найден. Необходимо зарегистрироваться"
                });
            }
                    
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user: {Email}", request.Email);
            return StatusCode(500, new { message = "Internal server error", detail = ex.Message });
        }
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.CurrentPassword) || string.IsNullOrWhiteSpace(request.NewPassword))
        {
            _logger.LogWarning("Change password request missing current or new password");
            return BadRequest(new { message = "Current and new passwords are required" });
        }

        if (!Guid.TryParse(request.UserId, out var userId))
        {
            return BadRequest(new { message = "Invalid user id" });
        }
        
        try
        {
            var tokens = await authService.ChangePasswordAsync(
                userId, 
                request.CurrentPassword, 
                request.NewPassword, 
                cancellationToken);
            
            _logger.LogInformation("Admin changed password for user {UserId}", request.UserId);
            
            return Ok(tokens);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized password change attempt");
            return Unauthorized(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation during password change");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during password change");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterUserRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await authService.RegisterAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed registration attempt for user: {Email}", request.Email);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for user: {Email}", request.Email);
            return StatusCode(500, new { message = "Internal server error", detail = ex.Message });
        }
    }
    
    [HttpPost("register-manager")]
    public async Task<IActionResult> RegisterManagerAsync([FromBody] RegisterUserRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await authService.RegisterManagerAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed registration attempt for user: {Email}", request.Email);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for user: {Email}", request.Email);
            return StatusCode(500, new { message = "Internal server error", detail = ex.Message });
        }
    }

    [HttpPost("block/{userId}")]
    public async Task<IActionResult> BlockUserAsync(
        Guid userId,
        [FromBody] BlockUserRequest request,
        CancellationToken cancellationToken = default)
    {
        await authService.BlockUserAsync(
            userId, 
            request.Reason, 
            request.ToTimeSpan(), 
            cancellationToken);
        
        return ApiOk( new {
            message = $"User {userId} blocked {request.Reason}",
            blockerUntil = DateTime.UtcNow.Add(request.ToTimeSpan())
        });
    }
    
    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync([FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(request.RefreshToken))
            return BadRequest(new { message = "Refresh token is required for logout" });
        
        try
        {
            await authService.RevokeRefreshTokenAsync(request.RefreshToken, cancellationToken);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Logout failed: {Message}", ex.Message);
            return Unauthorized(new { message = ex.Message });
        }
    }
}