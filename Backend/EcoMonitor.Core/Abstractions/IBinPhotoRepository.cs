using EcoMonitor.Core.Models;

namespace EcoMonitor.DataAccess.Repositories
{
    public interface IBinPhotoRepository
    {
        Task<IEnumerable<BinPhoto>> GetAllBinPhotosAsync();
        Task<BinPhoto> GetPhotoByIdAsync(Guid photoBinId);
        Task<BinPhoto> AddBinPhotoAsync(BinPhoto binPhoto);
        Task<Guid> DeleteBinPhotoAsync(Guid binPhotoId);
        Task<IEnumerable<BinPhoto>> GetPhotosInBoundsAsync(
            double north,
            double south,
            double east,
            double west);
    }
}
