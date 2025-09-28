using EcoMonitor.App.Services;
using EcoMonitor.Contracts.Contracts.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EcoMonitor.API.Controllers.Authorization;

[ApiController]
[Route("api/[controller]")]
public class AuthorizationController : ControllerBase
{
    private readonly IAuthService _authorizationService;
    private readonly ILogger<AuthorizationController> _logger;

    public AuthorizationController(
        IAuthService authorizationService,
        ILogger<AuthorizationController> logger)
    {
        _authorizationService = authorizationService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] AuthRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _authorizationService.LoginAsync(request, cancellationToken);
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
    
}