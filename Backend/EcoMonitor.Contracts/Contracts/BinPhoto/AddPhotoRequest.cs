using EcoMonitor.Contracts.Contracts.User;
using EcoMonitor.Contracts.Contracts.Users;
using NetTopologySuite.Geometries;

namespace EcoMonitor.Contracts.Contracts.BinPhoto
{
    public record AddPhotoRequest(
        //Guid Id,
        string FileName,
        string UrlFile,
        double Latitude,
        double Longitude,
        int District,
        DateTime UploadedAt,
        List<Guid> BinTypeId, 
        double FillLevel,
        bool IsOutsideBin,
        string Comment,
        int TotalBins,
        RegisterUserRequest UploadedBy,
        //UserRequest UploadedBy,
        Guid UploadedById);
}
