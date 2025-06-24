namespace EcoMonitor.Contracts.Contracts
{
    public record BinPhotoRequest(
        Guid Id,
        string FileName,
        string UrlFile,
        double Latitude,
        double Longitude,
        DateTime UploadedAt,
        string BinType,
        double FillLevel,
        bool IsOutsideBin,
        string Comment);
}
