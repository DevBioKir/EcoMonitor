namespace EcoMonitor.Contracts.Models;

public sealed record PhotoQuery
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string SortBy { get; init; } = "dateDesc";
    public int? District { get; init; }
    public bool? OnlyOutsideBin { get; init; }
    public double? MinFillLevel { get; init; }
    public double? MaxFillLevel { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public string? UploadedBySearch {get; init;}

    public bool IsValid(out string error)
    {
        if (Page < 1)
        {
            error = "Страница должна быть >= 1";
            return false;
        }

        if (PageSize is < 1 or > 100)
        {
            error = "Размер страницы должен быть в диапазоне от 1 до 100 объектов";
            return false;
        }
        
        error = String.Empty;
        return true;
    }
}