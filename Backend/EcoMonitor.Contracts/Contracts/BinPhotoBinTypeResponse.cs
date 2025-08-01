namespace EcoMonitor.Contracts.Contracts
{
    public record BinPhotoBinTypeResponse(
        Guid BinPhotoId,
        Guid BinTypeId,
        string BinTypeName);
}
