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

        Task<PagedResultDTO<BinPhotoResponse>> GetAllPhotosWithFilterAsync(
            PhotoQuery query,
            CancellationToken cancellationToken = default);
        Task<PagedResultDTO<BinPhotoResponse>> GetUserPhotosAsync(
            Guid userId,
            PhotoQuery query,
            CancellationToken cancellationToken = default);
        Task<int> GetCountPhotosAsync(CancellationToken cancellationToken = default);

        Task<IReadOnlyList<BinPhotoResponse>> GetLatestPhotosAsync(
            int count = 5,
            CancellationToken cancellationToken = default);
        Task<BinPhotoResponse> AddBinPhotoAsync(AddPhotoRequest requestAddBinPhoto);
        Task DeleteBinPhotoAsync(Guid binPhotoId);
        Task<BinPhotoResponse> UploadPhotoAsync(
            BinPhotoUploadRequest request,
            Guid userId,
            CancellationToken ct = default);

        Task UpdatePhotoAsync(
            Guid actorId,
            Guid photoId,
            UpdatePhotoRequest request,
            CancellationToken cancellationToken = default);
        Task<IEnumerable<BinPhotoResponse>> GetPhotosInBoundsAsync(
            double north,
            double south,
            double east,
            double west);

        // Task<BinPhotoMapResponse> GetByCoordinatesAsync(
        //     double latitude, 
        //     double longitude, 
        //     CancellationToken cancellationToken = default);
    }
}
