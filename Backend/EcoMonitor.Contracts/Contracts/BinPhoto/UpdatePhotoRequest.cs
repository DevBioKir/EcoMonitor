using EcoMonitor.Contracts.Contracts.User;
using EcoMonitor.Contracts.Contracts.Users;
using Microsoft.AspNetCore.Http;
using NetTopologySuite.Geometries;

namespace EcoMonitor.Contracts.Contracts.BinPhoto
{
    public record UpdatePhotoRequest(
        IFormFile? Photo,
        int? District,
        //DateTime? UploadedAt,
        List<Guid>? BinTypeId, 
        string? FillLevel,
        bool? IsOutsideBin,
        string? Comment,
        int? TotalBins);
}
