namespace EcoMonitor.Contracts.Contracts.BinPhotoBinType
{
    public record BinPhotoBinTypeResponse(
        Guid BinPhotoId,
        Guid BinTypeId,
        string BinTypeName);
}
