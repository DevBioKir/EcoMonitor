namespace EcoMonitor.Contracts.Contracts
{
    public record ProcessedImageResult(
        string OriginalUrl,
        // string LargeUrl,
        // string ThumbUrl,
        int OriginalWidth,
        int OriginalHeight,
        // int LargeWidth,
        // int LargeHeight,
        // int ThumbWidth,
        // int ThumbHeight,
        string SourceFormat,
        // DateTimeOffset? ShotAtUtc,
        (double lat, double lon)? Gps);
}
