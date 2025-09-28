using EcoMonitor.Contracts.Contracts.User;
using Microsoft.AspNetCore.Http;

namespace EcoMonitor.Contracts.Contracts.BinPhotoUpload
{
    public record BinPhotoUploadRequest(
        IFormFile Photo,
        List<Guid> BinTypeId,
        double FillLevel,
        bool IsOutsideBin,
        string Comment,
        UserRequest UploadedBy,
        Guid UploadedById);
}
