using Microsoft.AspNetCore.Http;

namespace EcoMonitor.Contracts.Contracts.BinPhotoUpload
{
    public record BinPhotoUploadRequestTest(
        IFormFile Photo,
        string BinType,
        double FillLevel,
        bool IsOutsideBin,
        string Comment);
}
