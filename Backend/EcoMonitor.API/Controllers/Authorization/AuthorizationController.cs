using System.Security.Claims;
using EcoMonitor.App.Services;
using EcoMonitor.Contracts.Contracts.Auth;
using EcoMonitor.Contracts.Contracts.Users;
using EcoMonitor.Core.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EcoMonitor.API.Controllers.Authorization;

[ApiController]
[Route("api/[controller]")]
public class AuthorizationController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthorizationController> _logger;

    public AuthorizationController(
        IAuthService authService,
        ILogger<AuthorizationController> logger)
    {
        _authService = authService;
        _logger = logger;
    }
    
    // private Guid GetCurrentUser()
    // {
    //     var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
    //     if (userIdClaim == null) 
    //         throw new UnauthorizedAccessException("User is not authenticated");
    //     
    //     var roleClaim = User.FindFirst(ClaimTypes.Role);
    //     if (roleClaim == null) 
    //         throw new UnauthorizedAccessException("authenticated user does not have a role");
    //     
    //     return Guid.Parse(userIdClaim.Value);
    // }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] AuthRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _authService.LoginAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Failed to login attempt for user: {Email}", request.Email);
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user: {Email}", request.Email);
            return StatusCode(500, new { message = "Internal server error", detail = ex.Message });
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterUserRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _authService.RegisterAsync(request, cancellationToken);
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
    
    [Authorize(Roles = "admin")]
    [HttpPost("register-admin")]
    public async Task<IActionResult> RegisterAdminAsync([FromBody] RegisterUserRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _authService.RegisterAdminAsync(request, cancellationToken);
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
    
    //[Authorize(Roles = "user")]
    //[Authorize(Roles = "admin, manager")]
    [HttpPost("register-manager")]
    public async Task<IActionResult> RegisterManagerAsync([FromBody] RegisterUserRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _authService.RegisterManagerAsync(request, cancellationToken);
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

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(request.RefreshToken))
        {
            _logger.LogWarning("Refresh token was not provided");
            return BadRequest(new { message = "Refresh token is required" });
        }
            
        try
        {
            var tokens = await _authService.RefreshTokenAsync(request.RefreshToken, cancellationToken);
            return Ok(tokens);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid or expired refresh token provided");
            return Unauthorized(new { message = "Invalid or expired refresh token" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during refresh token");
            return StatusCode(500, new { message = "Internal server error", detail = ex.Message });
        }
    }

    [Authorize(Roles = "admin")]
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
            var tokens = await _authService.ChangePasswordAsync(
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
    
    // [Authorize]
    // [HttpPost("change-password")]
    // public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordRequest request,
    //     CancellationToken cancellationToken = default)
    // {
    //     if (string.IsNullOrWhiteSpace(request.CurrentPassword) || string.IsNullOrWhiteSpace(request.NewPassword))
    //     {
    //         _logger.LogWarning("Change password request missing current or new password");
    //         return BadRequest(new { message = "Current and new passwords are required" });
    //     }
    //     
    //     var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
    //     if (userIdClaim == null)
    //         return Unauthorized(new { message = "User not found" });
    //     
    //     try
    //     {
    //         var userId = Guid.Parse(userIdClaim.Value);
    //         
    //         var tokens = await _authService.ChangePasswordAsync(
    //             userId, 
    //             request.CurrentPassword, 
    //             request.NewPassword, 
    //             cancellationToken);
    //         
    //         _logger.LogInformation("User {UserId} changed password successfully", userId);
    //         
    //         return Ok(tokens);
    //     }
    //     catch (UnauthorizedAccessException ex)
    //     {
    //         _logger.LogWarning(ex, "Unauthorized password change attempt for user {UserId}", userIdClaim.Value);
    //         return Unauthorized(new { message = ex.Message });
    //     }
    //     catch (InvalidOperationException ex)
    //     {
    //         _logger.LogWarning(ex, "Invalid operation during password change for user {UserId}", userIdClaim.Value);
    //         return BadRequest(new { message = ex.Message });
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Unexpected error during password change for user {UserId}", userIdClaim.Value);
    //         return StatusCode(500, new { message = "Internal server error" });
    //     }
    // }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync([FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(request.RefreshToken))
            return BadRequest(new { message = "Refresh token is required for logout" });
        
        await _authService.RevokeRefreshTokenAsync(request.RefreshToken, cancellationToken);
        return NoContent();
    }
}