using EcoMonitor.Core.Models;

namespace EcoMonitor.DataAccess.Repositories
{
    public interface IBinTypeRepository
    {
        Task<IEnumerable<BinType>> GetAllBinTypesAsync();
        Task<BinType> GetBinTypeByIdAsync(Guid binTypeId);
        Task<BinType> AddBinTypeAsync(BinType binType);
        Task<Guid> DeleteBinTypeAsync(Guid binTypeId);
    }
}