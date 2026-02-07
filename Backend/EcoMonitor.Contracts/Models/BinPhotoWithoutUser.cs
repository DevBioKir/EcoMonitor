namespace EcoMonitor.Contracts.Models;

public sealed record BinPhotoWithoutUser
{
    public Guid Id { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string UrlFile { get; init; } = string.Empty;
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public int District { get; init; }
    public DateTime UploadedAt { get; init; }
    public IEnumerable<Guid> BinTypeId { get; init; } = Enumerable.Empty<Guid>();
    public int FillLevel { get; init; }
    public bool IsOutsideBin { get; init; }
    public string Comment { get; init; } = string.Empty;
    public int TotalBins { get; init; }
}