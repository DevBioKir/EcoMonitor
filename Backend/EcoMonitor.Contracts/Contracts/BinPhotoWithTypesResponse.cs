namespace EcoMonitor.Contracts.Contracts
{
    public record BinPhotoWithTypesResponse(
       Guid Id,
       string FileName,
       string UrlFile,
       double Latitude,
       double Longitude,
       DateTime UploadedAt,
       double FillLevel,
       bool IsOutsideBin,
       string Comment,
       List<BinTypeResponse> BinTypes);
}
