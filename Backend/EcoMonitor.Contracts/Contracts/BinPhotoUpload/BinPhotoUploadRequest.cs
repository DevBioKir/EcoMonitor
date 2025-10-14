using EcoMonitor.Contracts.Contracts.User;
using Microsoft.AspNetCore.Http;

namespace EcoMonitor.Contracts.Contracts.BinPhotoUpload
{
    public record BinPhotoUploadRequest(
        IFormFile Photo,
        List<string> BinTypeCode,
        double FillLevel,
        bool IsOutsideBin,
        string Comment,
        int TotalBins,
        UserRequest UploadedBy,
        Guid UploadedById);
}
