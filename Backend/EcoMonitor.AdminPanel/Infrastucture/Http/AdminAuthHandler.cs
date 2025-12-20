using System.Net.Http.Headers;
using Microsoft.JSInterop;

namespace EcoMonitor.AdminPanel.Infrastucture.Http;

public class AdminAuthHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AdminAuthHandler> _logger;

    public AdminAuthHandler(
        IHttpContextAccessor httpContextAccessor, 
        ILogger<AdminAuthHandler> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, 
        CancellationToken cancellationToken)
    {
        var context = _httpContextAccessor.HttpContext;

        var token = context?.Request.Cookies["accessToken"];
        
        _logger.LogInformation("AdminAuthHandler: {Method} {Url}", request.Method, request.RequestUri);
        _logger.LogInformation("Token from cookie: '{Token}'", token ?? "<null>");

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        _logger.LogInformation("Authorization header: {Auth}",
            request.Headers.Authorization?.ToString() ?? "<null>");

        return await base.SendAsync(request, cancellationToken);
    }
}