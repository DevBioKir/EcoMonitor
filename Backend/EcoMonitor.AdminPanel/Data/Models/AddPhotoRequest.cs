namespace EcoMonitor.AdminPanel.Data.Models;

public record AddPhotoRequest
{
    public string? Photo { get; set; }
    public List<string> BinTypeCode { get; init; }
    public double FillLevel { get; init; }
    public bool IsOutsideBin { get; init; }
    public string Comment { get; init; }
    public int TotalBins { get; init; }
}