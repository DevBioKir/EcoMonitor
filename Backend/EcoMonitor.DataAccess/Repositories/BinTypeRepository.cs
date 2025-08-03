using EcoMonitor.Core.Models;
using EcoMonitor.DataAccess.Entities;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace EcoMonitor.DataAccess.Repositories
{
    public class BinTypeRepository : IBinTypeRepository
    {
        private readonly EcoMonitorDbContext _context;
        private readonly IMapper _mapper;

        public BinTypeRepository(
            EcoMonitorDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<BinType> AddBinTypeAsync(BinType binType)
        {
            var binTypeEntity = _mapper.Map<BinTypeEntity>(binType);

            await _context.BinTypes.AddAsync(binTypeEntity);
            await _context.SaveChangesAsync();

            return binType;
        }

        public async Task<Guid> DeleteBinTypeAsync(Guid binTypeId)
        {
            var binTypeEntity = await _context.BinTypes
                .FirstOrDefaultAsync(bt => bt.Id == binTypeId);

            if (binTypeEntity == null)
                throw new NullReferenceException($"Container type with id {binTypeId} not found");
            
            _context.BinTypes.Remove(binTypeEntity);
            await _context.SaveChangesAsync();

            return binTypeId;
        }

        public async Task<IEnumerable<BinType>> GetAllBinTypesAsync()
        {
            var binTypeEntities = await _context.BinTypes.ToListAsync();

            return _mapper.Map<IEnumerable<BinType>>(binTypeEntities);
        }

        public async Task<BinType> GetBinTypeByIdAsync(Guid binTypeId)
        {
            var binTypeEntity = await _context.BinTypes.
                FirstOrDefaultAsync(bt => bt.Id == binTypeId);

            if (binTypeEntity == null)
                throw new NullReferenceException($"Container type with id {binTypeId} not found");

            return _mapper.Map<BinType>(binTypeEntity);
        }
    }
}
