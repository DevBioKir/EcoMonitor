using EcoMonitor.Contracts.Contracts;
using EcoMonitor.Infrastracture.Abstractions;
using EcoMonitor.Infrastracture.Services;
using HeyRed.ImageSharp.Heif.Formats.Avif;
using HeyRed.ImageSharp.Heif.Formats.Heif;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace EcoMonitor.Infrastracture.Pipeline
{
    public class ImagePipeline : IImagePipeline
    {
        private readonly IImageStorageService _storageService;
        private readonly IGeolocationService _geolocationService;
        private readonly ILogger<ImagePipeline> _logger;

        public ImagePipeline(
            IImageStorageService storageService, 
            IGeolocationService geolocationService,
            ILogger<ImagePipeline> logger)
        {
            _storageService = storageService;
            _geolocationService = geolocationService;
            _logger = logger;
        }

        public async Task<ProcessedImageResult> ProcessAsync(IFormFile file, CancellationToken ct)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (file.Length == 0) throw new InvalidOperationException("File is empty");

            await using var ms = new MemoryStream();
            await file.CopyToAsync(ms, ct);
            ms.Position = 0;

            var decoderOptions = new DecoderOptions()
            {
                Configuration = new Configuration(
                new AvifConfigurationModule(),
                new HeifConfigurationModule(),
                new JpegConfigurationModule(),
                new PngConfigurationModule())
            };
            if (decoderOptions == null) throw new InvalidOperationException("ImageSharp configuration is null");

            try
            {
                using var uploadedImage = Image.Load<Rgba32>(decoderOptions, ms);

                var exif = uploadedImage.Metadata.ExifProfile;
                var (lat, lon) = _geolocationService.GeoLocationService(exif);

                var url = await _storageService.SaveImageAsync(file);

                return new ProcessedImageResult(
                    OriginalUrl: url,
                    OriginalWidth: uploadedImage.Width,
                    OriginalHeight: uploadedImage.Height,
                    SourceFormat: uploadedImage.Metadata.DecodedImageFormat?.Name ?? "unknown",
                    Gps: (lat ?? 0, lon ?? 0)
                );
            }
            catch (UnknownImageFormatException ex)
            {
                _logger.LogError(ex, "Unsupported image format");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load image");
                throw;
            }
        }
    }
}
