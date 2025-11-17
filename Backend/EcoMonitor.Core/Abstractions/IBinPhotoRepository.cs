using EcoMonitor.Contracts.Models;
using EcoMonitor.Core.Models;

namespace EcoMonitor.DataAccess.Repositories
{
    public interface IBinPhotoRepository
    {
        Task<IReadOnlyList<PhotoMarker>> GetMarkersAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<BinPhoto>> GetAllBinPhotosAsync();
        Task<BinPhoto> GetPhotoByIdAsync(Guid photoBinId);
        Task<PagedResult<BinPhoto>> GetUserPhotosAsync(
            Guid userId,
            PhotoQuery query,
            CancellationToken cancellationToken = default);
        Task<BinPhoto> AddBinPhotoAsync(BinPhoto binPhoto);
        Task<Guid> DeleteBinPhotoAsync(Guid binPhotoId);
        Task<IReadOnlyList<BinPhoto>> GetPhotosInBoundsAsync(
            double north,
            double south,
            double east,
            double west);
    }
}
