using Microsoft.AspNetCore.Http;

namespace EcoMonitor.Infrastracture.Abstractions
{
    public interface IImageValidator
    {
        Task ValidateAsync(IFormFile file, CancellationToken ct);
    }
}