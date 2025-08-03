using Microsoft.AspNetCore.Http;

namespace EcoMonitor.Contracts.Contracts
{
    public record BinPhotoUploadRequest(
        IFormFile Photo,
        List<Guid> BinTypeId,
        double FillLevel,
        bool IsOutsideBin,
        string Comment);
}
