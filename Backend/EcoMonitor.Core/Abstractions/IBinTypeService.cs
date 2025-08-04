using EcoMonitor.Contracts.Contracts;

namespace EcoMonitor.App.Services
{
    public interface IBinTypeService
    {
        Task<IReadOnlyList<BinTypeResponse>> GetAllBinTypesAsync();
        Task<BinTypeResponse> GetBinTypeByIdAsync(Guid binTypeId);
        Task<BinTypeResponse> AddBinTypeAsync(BinTypeRequest requestBinType);
        Task<Guid> DeleteBinTypeAsync(Guid binTypeId);
    }
}