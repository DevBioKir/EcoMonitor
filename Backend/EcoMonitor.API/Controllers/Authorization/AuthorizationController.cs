using EcoMonitor.App.Services;
using EcoMonitor.Contracts.Contracts.Auth;
using EcoMonitor.Contracts.Contracts.Users;
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
    
}