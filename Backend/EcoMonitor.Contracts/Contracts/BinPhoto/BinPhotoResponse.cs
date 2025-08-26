namespace EcoMonitor.Contracts.Contracts.BinPhoto
{
    public record BinPhotoResponse(
       Guid Id,
       string FileName,
       string UrlFile,
       double Latitude,
       double Longitude,
       DateTime UploadedAt,
       List<Guid> BinTypeId,
       double FillLevel,
       bool IsOutsideBin,
       string Comment);
}
