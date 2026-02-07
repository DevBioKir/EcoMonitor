namespace EcoMonitor.Contracts.Contracts.BinPhoto;

public record BinPhotoMapResponse(
    Guid Id,
    string FileName,
    string UrlFile,
    double Latitude, 
    double Longitude,
    string District,
    DateTime UploadedAt,
    string Comment);