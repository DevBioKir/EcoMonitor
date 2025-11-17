using EcoMonitor.Contracts.Contracts;
using EcoMonitor.Contracts.Contracts.BinPhoto;
using EcoMonitor.Contracts.Contracts.BinPhotoUpload;
using EcoMonitor.Contracts.Models;
namespace EcoMonitor.App.Services
{
    public interface IBinPhotoService
    {
        Task<IReadOnlyList<PhotoMarkerDTO>> GetMarkersAsync();
        Task<IReadOnlyList<BinPhotoResponse>> GetAllBinPhotosAsync();
        Task<BinPhotoResponse> GetPhotoByIdAsync(Guid photoBinId);
        Task<PagedResultDTO<BinPhotoResponse>> GetUserPhotosAsync(
            Guid userId,
            PhotoQuery query,
            CancellationToken cancellationToken = default);
        Task<BinPhotoResponse> AddBinPhotoAsync(BinPhotoRequest requestBinPhoto);
        Task<Guid> DeleteBinPhotoAsync(Guid binPhotoId);
        Task<BinPhotoResponse> UploadImage(
            BinPhotoUploadRequest request,
            Guid userId,
            CancellationToken ct = default);
        Task<IEnumerable<BinPhotoResponse>> GetPhotosInBoundsAsync(
            double north,
            double south,
            double east,
            double west);
    }
}
