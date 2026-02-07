using EcoMonitor.API.Attributes;
using EcoMonitor.App.Services;
using EcoMonitor.Contracts.Contracts.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoMonitor.API.Controllers.Authorization;

[PublicApi]
[Authorize]
[Route("[controller]")]
public class AuthorizationController(
    IAuthService authService,
    ILogger<AuthorizationController> _logger)
    : BaseApiController<AuthorizationController>(_logger)
{
    [HttpPost("Validate")]
    public IActionResult ValidateToken() => Ok(new {message = "Token is valid"});

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] AuthRequest request,
        CancellationToken cancellationToken = default)
    {
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
    
    [AllowAnonymous]
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
            var tokens = await authService.RefreshTokenAsync(request.RefreshToken, cancellationToken);
            return Ok(tokens);
        }
        // catch (InvalidOperationException ex)
        // {
        //     _logger.LogWarning(ex, "Invalid or expired refresh token provided");
        //     return Unauthorized(new { message = "Invalid or expired refresh token" });
        // }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during refresh token");
            return StatusCode(500, new { message = "Internal server error", detail = ex.Message });
        }
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