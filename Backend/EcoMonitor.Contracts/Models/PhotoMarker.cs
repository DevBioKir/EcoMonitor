namespace EcoMonitor.Contracts.Models;

public sealed record PhotoMarker
{
    public Guid Id { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public string PhotoUrl { get; set; }
}