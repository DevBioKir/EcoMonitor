using EcoMonitor.Contracts.Contracts;
using EcoMonitor.Core.Models;

namespace EcoMonitor.App.Services
{
    public interface IBinPhotoService
    {
        Task<IReadOnlyList<BinPhotoResponse>> GetAllBinPhotosAsync();
        Task<BinPhotoResponse> GetPhotoByIdAsync(Guid photoBinId);
        Task<BinPhotoResponse> AddBinPhotoAsync(BinPhotoRequest requestBinPhoto);
        Task<Guid> DeleteBinPhotoAsync(Guid binPhotoId);
        Task<BinPhotoResponse> UploadImage(BinPhotoUploadRequest request);
    }
}
