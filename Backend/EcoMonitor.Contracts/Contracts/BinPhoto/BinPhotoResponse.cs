using EcoMonitor.Contracts.Contracts.User;
using EcoMonitor.Contracts.Contracts.Users;

namespace EcoMonitor.Contracts.Contracts.BinPhoto
{
    public record BinPhotoResponse(
        Guid Id,
        string FileName,
        string UrlFile,
        double Longitude,
        double Latitude,
        string District,
        DateTime UploadedAt,
        List<Guid> BinTypeId,
        double FillLevel,
        bool IsOutsideBin,
        int TotalBins,
        string Comment,
        UserResponse UploadedBy);
    //Guid UploadedById);
}
