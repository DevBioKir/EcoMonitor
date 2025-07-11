using Microsoft.AspNetCore.Http;

namespace EcoMonitor.Infrastracture.Abstractions
{
    public interface IImageStorageService
    {
        Task<string> SaveImageAsync(IFormFile image);
    }
}