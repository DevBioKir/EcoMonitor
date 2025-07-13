using EcoMonitor.Infrastracture.Abstractions;
using Microsoft.AspNetCore.Http;

namespace EcoMonitor.Infrastracture.Services
{
    public class ImageStorageService : IImageStorageService
    {
        private readonly string _webRootPath;

        public ImageStorageService(string webRootPath)
        {
            _webRootPath = webRootPath;
        }
        public async Task<string> SaveImageAsync(IFormFile photo)
        {
            if (photo == null || photo.Length < 0)
                throw new ArgumentException("Invalid file");

            if(!photo.ContentType.StartsWith("image/"))
                throw new ArgumentException("Not an image");

            var photoFolder = Path.Combine(_webRootPath, "Photos");

            if(!Directory.Exists(photoFolder))
                Directory.CreateDirectory(photoFolder);

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
