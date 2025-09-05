using EcoMonitor.Core.Models;
using EcoMonitor.DataAccess.Entities;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace EcoMonitor.DataAccess.Repositories
{
    public class BinPhotoRepository : IBinPhotoRepository
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

        public async Task<IEnumerable<BinPhoto>> GetAllBinPhotosAsync()
        {
            var binPhotosEntity = await _context.BinPhotos
                .Include(bp => bp.BinPhotoBinTypes)
                .ThenInclude(bbt => bbt.BinType)
                .ToListAsync();

            var binPhotos = _mapper.Map<IEnumerable<BinPhoto>>(binPhotosEntity);

            return binPhotos;
        }

        public async Task<IEnumerable<BinPhoto>> GetPhotosInBoundsAsync(
            double north,
            double south,
            double east,
            double west)
        {
            var photos = await _context.BinPhotos
                .Where(b => b.Location.Y <= north && b.Location.Y >= south
                && b.Location.X >= west && b.Location.X <= east)
                .Select(b => new
                {
                    b.FileName,
                    b.Location.Y,
                    b.Location.X,
                    b.FillLevel
                }).ToListAsync();

            return _mapper.Map<IEnumerable<BinPhoto>>(photos);
        }

        public async Task<BinPhoto> AddBinPhotoAsync(
            BinPhoto binPhoto)
        {
            var binPhotoEntity = _mapper.Map<BinPhotoEntity>(binPhoto);

            await _context.BinPhotos.AddAsync(binPhotoEntity);
            await _context.SaveChangesAsync();

            return binPhoto;
        }

        public async Task<BinPhoto> GetPhotoByIdAsync(Guid photoBinId)
        {
            var entityBinPhoto = await _context.BinPhotos
                .Include(bp => bp.BinPhotoBinTypes)
                .ThenInclude(bbt => bbt.BinType)
                .FirstOrDefaultAsync(b => b.Id == photoBinId);

            if (entityBinPhoto == null)
                throw new NullReferenceException($"Container photo with ID {photoBinId} not found");

            return _mapper.Map<BinPhoto>(entityBinPhoto);
        }

        public async Task<Guid> DeleteBinPhotoAsync(Guid binPhotoId)
        {
                var binPhotoEntity = await _context.BinPhotos
                .FirstOrDefaultAsync(b => b.Id == binPhotoId);

                if (binPhotoEntity == null)
                    throw new NullReferenceException($"Container photo with ID {binPhotoId} not found");

                _context.BinPhotos.Remove(binPhotoEntity);
                await _context.SaveChangesAsync();

                //var deletedBinPhoto = await _context.BinPhotos
                //        .Where(b => b.Id == binPhotoId)
                //        .ExecuteDeleteAsync();

                //if (deletedBinPhoto == 0)
                //    throw new NullReferenceException($"Container photo with ID {binPhotoId} not found");

                return binPhotoId;
        }
    }
}
