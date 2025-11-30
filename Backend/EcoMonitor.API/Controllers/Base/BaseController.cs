using Microsoft.AspNetCore.Mvc;

namespace EcoMonitor.API.Controllers;

public abstract class BaseApiController<T> : ControllerBase
{
    protected readonly ILogger<T> _logger;

    protected BaseApiController(
        ILogger<T> logger)
    {
        _logger = logger;
    }
    
    protected IActionResult ApiOk<TResponse>(TResponse response) 
        => Ok(new {success = true, data = response});
    
    protected IActionResult ApiError(string message) 
        => BadRequest(new {success = false, error = message});
}