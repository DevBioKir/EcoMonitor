namespace EcoMonitor.AdminPanel.Data.Models;

public class AddPhotoRequest
{
    public string Photo { get; set; } = "";
    public int District { get; set; }
    public List<string> BinTypeCode { get; set; } = new();
    public double FillLevel { get; set; }
    public bool IsOutsideBin { get; set; }
    public string Comment { get; set; }
    public int TotalBins { get; set; }
}