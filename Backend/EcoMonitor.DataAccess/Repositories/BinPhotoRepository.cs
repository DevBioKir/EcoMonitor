using EcoMonitor.Contracts.Contracts;
using EcoMonitor.Contracts.Models;
using EcoMonitor.Core.Models;
using EcoMonitor.DataAccess.Entities;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace EcoMonitor.DataAccess.Repositories
{
    public class BinPhotoRepository(
        EcoMonitorDbContext context,
        IMapper mapper) : IBinPhotoRepository
    {
        public async Task<IReadOnlyList<BinPhoto>> GetAllBinPhotosAsync()
        {
            var binPhotosEntity = await context.BinPhotos
                .Include(bp => bp.BinPhotoBinTypes)
                    .ThenInclude(bbt => bbt.BinType)
                .Include(bp => bp.UploadedBy)
                    .ThenInclude(u => u.Role)
                        .ThenInclude(r => r.Permissions)
                //.AsNoTracking()
                .ToListAsync();

            var binPhotos = mapper.Map<List<BinPhoto>>(binPhotosEntity);

            return binPhotos;
        }

        public async Task<IReadOnlyList<BinPhoto>> GetPhotosInBoundsAsync(
            double north,
            double south,
            double east,
            double west)
        {
            var photos = await context.BinPhotos
                .Where(b => b.Location.Y <= north && b.Location.Y >= south
                && b.Location.X >= west && b.Location.X <= east)
                .Select(b => new
                {
                    b.FileName,
                    b.Location.Y,
                    b.Location.X,
                    b.FillLevel
                }).ToListAsync();

            return mapper.Map<List<BinPhoto>>(photos);
        }
        
        Task<Contracts.Models.PagedResult<BinPhoto>> GetUserPhotosAsync(
            Guid userId,
            PhotoQuery query,
            CancellationToken cancellationToken = default)
        {
            // var photos = await context.BinPhotos
            //     .Include(bp => bp.BinPhotoBinTypes)
            //         .ThenInclude(bbt => bbt.BinType)
            //     .Include(bp => bp.UploadedBy)
            //         .ThenInclude(u => u.Role)
            //         .ThenInclude(r => r.Permissions)
            //     .Where(bp => bp.UploadedBy.Id == userId)
            //     .ToListAsync();
            //
            // var binPhotos = mapper.Map<List<BinPhoto>>(photos);

            var query = context.BinPhotos
                .Where(bp => bp.Id == userId);
            query = ApplyFilters(query, userId);
            
            return binPhotos;
        }

        public async Task<BinPhoto> AddBinPhotoAsync(
            BinPhoto binPhoto)
        {
            var binPhotoEntity = mapper.Map<BinPhotoEntity>(binPhoto);

            await context.BinPhotos.AddAsync(binPhotoEntity);
            await context.SaveChangesAsync();

            return binPhoto;
        }

        public async Task<BinPhoto> GetPhotoByIdAsync(Guid photoBinId)
        {
            var entityBinPhoto = await context.BinPhotos
                .Include(bp => bp.BinPhotoBinTypes)
                    .ThenInclude(bbt => bbt.BinType)
                .Include(bp => bp.UploadedBy)
                    .ThenInclude(u => u.Role)
                    .ThenInclude(r => r.Permissions)
                .FirstOrDefaultAsync(b => b.Id == photoBinId);

            if (entityBinPhoto == null)
                throw new NullReferenceException($"Container photo with ID {photoBinId} not found");

            return mapper.Map<BinPhoto>(entityBinPhoto);
        }

        public async Task<Guid> DeleteBinPhotoAsync(Guid binPhotoId)
        {
                var binPhotoEntity = await context.BinPhotos
                .FirstOrDefaultAsync(b => b.Id == binPhotoId);

                if (binPhotoEntity == null)
                    throw new NullReferenceException($"Container photo with ID {binPhotoId} not found");

                context.BinPhotos.Remove(binPhotoEntity);
                await context.SaveChangesAsync();

                //var deletedBinPhoto = await _context.BinPhotos
                //        .Where(b => b.Id == binPhotoId)
                //        .ExecuteDeleteAsync();

                //if (deletedBinPhoto == 0)
                //    throw new NullReferenceException($"Container photo with ID {binPhotoId} not found");

                return binPhotoId;
        }
        
        private IQueryable<BinPhotoEntity> ApplyFilters(
            IQueryable<BinPhotoEntity> query,
            PhotoFilter filters)
        {
            if (filters.OnlyOutsideBin.HasValue && filters.OnlyOutsideBin.Value)
            {
                query = query.Where(p => p.IsOutsideBin);
            }

            if (filters.MinFillLevel.HasValue)
            {
                query = query.Where(p => p.FillLevel >= filters.MinFillLevel.Value);
            }

            if (filters.MaxFillLevel.HasValue)
            {
                query = query.Where(p => p.FillLevel <= filters.MaxFillLevel.Value);
            }

            if (filters.FromDate.HasValue)
            {
                query = query.Where(p => p.UploadedAt >= filters.FromDate.Value);
            }

            if (filters.ToDate.HasValue)
            {
                query = query.Where(p => p.UploadedAt <= filters.ToDate.Value);
            }
            
            return query;
        }
    }
}
