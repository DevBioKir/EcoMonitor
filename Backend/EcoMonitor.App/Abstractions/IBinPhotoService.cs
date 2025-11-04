using EcoMonitor.App.Models.Queries;
using EcoMonitor.Contracts.Contracts;
using EcoMonitor.Contracts.Contracts.BinPhoto;
using EcoMonitor.Contracts.Contracts.BinPhotoUpload;


namespace EcoMonitor.App.Services
{
    public interface IBinPhotoService
    {
        Task<IReadOnlyList<BinPhotoResponse>> GetAllBinPhotosAsync();
        Task<BinPhotoResponse> GetPhotoByIdAsync(Guid photoBinId);

        Task<PagedResult<BinPhotoResponse>> GetUserPhotosAsync(
            Guid userId,
            PhotoQuery query,
            CancellationToken cancellationToken = default);
        Task<BinPhotoResponse> AddBinPhotoAsync(BinPhotoRequest requestBinPhoto);
        Task<Guid> DeleteBinPhotoAsync(Guid binPhotoId);
        Task<BinPhotoResponse> UploadImage(BinPhotoUploadRequest request, CancellationToken ct);
        Task<IEnumerable<BinPhotoResponse>> GetPhotosInBoundsAsync(
            double north,
            double south,
            double east,
            double west);
    }
}
