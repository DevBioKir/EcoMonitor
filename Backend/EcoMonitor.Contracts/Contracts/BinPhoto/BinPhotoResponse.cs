using EcoMonitor.Contracts.Contracts.User;

namespace EcoMonitor.Contracts.Contracts.BinPhoto
{
    public record BinPhotoResponse(
       Guid Id,
       string FileName,
       string UrlFile,
       string Location,
       //double Latitude,
       //double Longitude,
       DateTime UploadedAt,
       List<Guid> BinTypeId,
       double FillLevel,
       bool IsOutsideBin,
       string Comment,
       UserResponse UploadedBy);
}
