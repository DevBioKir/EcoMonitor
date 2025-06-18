using EcoMonitor.Core.Models;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace EcoMonitor.DataAccess.Repositories
{
    public class BinPhotoRepository
    {
        private readonly EcoMonitorDbContext _context;
        private readonly IMapper _mapper; 

        public BinPhotoRepository(
            EcoMonitorDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<BinPhoto> GetPhotoByIdAsync(Guid photoBinId)
        {
            var entityBinPhoto = await _context.BinPhotos
                .FirstOrDefaultAsync(b => b.Id == photoBinId);

            return _mapper.Map<BinPhoto>(entityBinPhoto);
        }
    }
}
