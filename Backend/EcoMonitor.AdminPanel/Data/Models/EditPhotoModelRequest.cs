using Microsoft.AspNetCore.Components.Forms;

namespace EcoMonitor.AdminPanel.Data.Models;

public class EditPhotoModelRequest
{
    public Guid Id { get; set; }
    public IBrowserFile? Photo { get; set; }
    public List<Guid>? BinTypeId { get; set; }
    public double? FillLevel { get; set; }
    public bool? IsOutsideBin { get; set; }
    public string? Comment { get; set; }
    public int? TotalBins { get; set; }
}
