using EcoMonitor.Contracts.Contracts.User;
using EcoMonitor.Contracts.Contracts.Users;
using NetTopologySuite.Geometries;

namespace EcoMonitor.Contracts.Contracts.BinPhoto
{
    public record BinPhotoRequest(
        Guid Id,
        string FileName,
        string UrlFile,
        double Latitude,
        double Longitude,
        DateTime UploadedAt,
        List<Guid> BinTypeId, 
        double FillLevel,
        bool IsOutsideBin,
        string Comment,
        RegisterUserRequest UploadedBy,
        //UserRequest UploadedBy,
        Guid UploadedById);
}
