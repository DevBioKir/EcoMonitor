using Microsoft.AspNetCore.Http;

namespace EcoMonitor.Contracts.Contracts
{
    public record BinPhotoUploadRequest(
        IFormFile Photo,
        //string FileName,
        //string UrlFile,
        //double Latitude,
        //double Longitude,
        //DateTime UploadedAt,
        string BinType,
        double FillLevel,
        bool IsOutsideBin,
        string Comment);
}
