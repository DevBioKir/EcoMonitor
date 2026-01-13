using EcoMonitor.Infrastracture.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace EcoMonitor.Infrastracture.Services
{
    public class ImageStorageService(
        string webRootPath, 
        ILogger<ImageStorageService> logger) : IImageStorageService
    {
        public async Task<string> SaveImageAsync(IFormFile photo)
        {
            if (photo == null || photo.Length == 0)
                throw new ArgumentException("Invalid file");

            // if(!photo.ContentType.StartsWith("image/"))
            //     throw new ArgumentException("Not an image");

            var photoFolder = Path.Combine(webRootPath, "Photos");

            if(!Directory.Exists(photoFolder))
                Directory.CreateDirectory(photoFolder);
            //
            var fileExtension = Path.GetExtension(photo.FileName).ToLowerInvariant();
            var supported = new[] {".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp", ".heic", ".heif"};
            
            if (!supported.Contains(fileExtension))
                logger?.LogWarning("Unknown extension {Ext}, saving anyway", fileExtension);
            //

            var uniqueName = $"{Guid.NewGuid()}_{Path.GetFileName(photo.FileName)}";
            var filePath = Path.Combine(photoFolder, uniqueName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await photo.CopyToAsync(stream);
            }

            return Path.Combine("Photos", uniqueName);
        }
    }
}
