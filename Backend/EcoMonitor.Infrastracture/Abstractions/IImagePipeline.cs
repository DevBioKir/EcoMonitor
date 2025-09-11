using EcoMonitor.Contracts.Contracts;
using Microsoft.AspNetCore.Http;

namespace EcoMonitor.Infrastracture.Abstractions
{
    public interface IImagePipeline
    {
        Task<ProcessedImageResult> ProcessAsync(IFormFile file, CancellationToken ct = default);
    }
}