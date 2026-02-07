namespace EcoMonitor.Contracts.Contracts;

public record PhotoFilterDTO
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string SortBy { get; init; } = "dateDesc";
    public string District { get; init; }
    public bool? OnlyOutsideBin { get; init; }
    public double? MinFillLevel { get; init; }
    public double? MaxFillLevel { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public string? UploadedBySearch { get; init; }
}