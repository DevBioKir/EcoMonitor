using EcoMonitor.Infrastracture.Abstractions;
using Microsoft.AspNetCore.Http;

namespace EcoMonitor.Infrastracture.Validate
{
    public class ImageValidator : IImageValidator
    {
        private static readonly HashSet<string> Allowed = new(StringComparer.OrdinalIgnoreCase)
        {
            "image/heic", "image/heif", "image/avif", "image/jpeg", "image/png", "image/webp"
        };
        public async Task ValidateAsync(IFormFile file, CancellationToken ct)
        {
            if (file.Length == 0) throw new("Empty file");
            if (!Allowed.Contains(file.ContentType)) throw new("Unsupported Content-Type");

            using var s = file.OpenReadStream();
            byte[] header = new byte[12];
            var read = await s.ReadAsync(header, ct);

            if (read < 4) throw new("Too small");



        }
    }
}
