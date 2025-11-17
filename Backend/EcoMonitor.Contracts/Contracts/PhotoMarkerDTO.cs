namespace EcoMonitor.Contracts.Contracts;

public sealed record PhotoMarkerDTO
{
    public Guid Id { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public string PhotoUrl { get; init; }
}