using EcoMonitor.Core.Models;

namespace EcoMonitor.DataAccess.Repositories
{
    public interface IBinPhotoRepository
    {
        Task<ICollection<BinPhoto>> GetAllBinPhotosAsync();
        Task<BinPhoto> GetPhotoByIdAsync(Guid photoBinId);
        Task<BinPhoto> AddBinPhotoAsync(BinPhoto binPhoto);
        Task<Guid> DeleteBinPhotoAsync(Guid binPhotoId);
    }
}
