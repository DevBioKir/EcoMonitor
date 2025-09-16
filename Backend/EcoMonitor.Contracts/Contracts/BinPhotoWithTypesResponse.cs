using EcoMonitor.Contracts.Contracts.BinType;

namespace EcoMonitor.Contracts.Contracts
{
    public record BinPhotoWithTypesResponse(
       Guid Id,
       string FileName,
       string UrlFile,
       //string Location,
       double Longitude,
       double Latitude,
       DateTime UploadedAt,
       double FillLevel,
       bool IsOutsideBin,
       string Comment,
       List<BinTypeResponse> BinTypes);
}
