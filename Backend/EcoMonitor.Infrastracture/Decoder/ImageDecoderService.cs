using EcoMonitor.Infrastracture.Abstractions;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

namespace EcoMonitor.Infrastracture.Decoder
{
    public class ImageDecoderService : IImageDecoderService
    {
        public async Task<Image> LoadAsync(IFormFile file, Configuration configuration, CancellationToken ct)
        {
            await using var ms = new MemoryStream();
            await file.CopyToAsync(ms, ct);
            ms.Position = 0;

            var decoderOptions = new DecoderOptions()
            {
                Configuration = configuration,
                SkipMetadata = false
            };

            return Image.Load(decoderOptions, ms);
        }
    }
}
