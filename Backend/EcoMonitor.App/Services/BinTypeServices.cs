using EcoMonitor.Contracts.Contracts;
using EcoMonitor.Core.Models;
using EcoMonitor.DataAccess.Repositories;
using MapsterMapper;
using Microsoft.Extensions.Logging;

namespace EcoMonitor.App.Services
{
    public class BinTypeService : IBinTypeService
    {
        private readonly IBinTypeRepository _binTypeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BinTypeService> _logger;
        public BinTypeService(
            IBinTypeRepository binTypeRepository, 
            IMapper mapper,
            ILogger<BinTypeService> logger)
        {
            _binTypeRepository = binTypeRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<BinTypeResponse> AddBinTypeAsync(BinTypeRequest requestBinType)
        {
            var domainBinType = _mapper.Map<BinType>(requestBinType);
            var addBinType = await _binTypeRepository.AddBinTypeAsync(domainBinType);

            return _mapper.Map<BinTypeResponse>(addBinType);
        }

        public async Task<Guid> DeleteBinTypeAsync(Guid binTypeId)
        {
            return await _binTypeRepository.DeleteBinTypeAsync(binTypeId);
        }

        public async Task<IReadOnlyList<BinTypeResponse>> GetAllBinTypesAsync()
        {
            var listBinType = await _binTypeRepository.GetAllBinTypesAsync();
            return _mapper.Map<IReadOnlyList<BinTypeResponse>>(listBinType);
        }

        public async Task<BinTypeResponse> GetBinTypeByIdAsync(Guid binTypeId)
        {
            var domainBinType = await _binTypeRepository.GetBinTypeByIdAsync(binTypeId);
            return _mapper.Map<BinTypeResponse>(domainBinType);
        }
    }
}
