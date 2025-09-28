using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;

namespace EcoMonitor.Infrastracture.Abstractions
{
    public interface IImageDecoderService
    {
        Task<Image> LoadAsync(IFormFile file, Configuration configuration, CancellationToken ct);
    }
}