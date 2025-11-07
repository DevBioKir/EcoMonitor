using EcoMonitor.Contracts.Contracts;
using EcoMonitor.Contracts.Models;
using EcoMonitor.Core.Models;
using EcoMonitor.DataAccess.Entities;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EcoMonitor.DataAccess.Repositories
{
    public class BinPhotoRepository(
        EcoMonitorDbContext context,
        IMapper mapper,
        ILogger<BinPhotoRepository> _logger) : IBinPhotoRepository
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
        
        public async Task<PagedResult<BinPhoto>> GetUserPhotosAsync(
            Guid userId,
            PhotoQuery query,
            CancellationToken cancellationToken = default)
        {
            var baseQuery = context.BinPhotos
                .AsNoTracking()
                .Where(bp => bp.UploadedById == userId);

            var filteredQuery = ApplyFilters(baseQuery, query);

            int totalCount = await filteredQuery.CountAsync(cancellationToken);
            _logger.LogInformation("Total count after filters: {TotalCount}", totalCount);

            if (totalCount == 0)
            {
                _logger.LogInformation("No photos found, returning empty list.");
                return new PagedResult<BinPhoto>(
                    new List<BinPhoto>(), 
                    0, 
                    query.Page, 
                    query.PageSize);
            }

            IQueryable<BinPhotoEntity> queryWithIncludes = filteredQuery
                .AsSplitQuery()
                .Include(p => p.BinPhotoBinTypes)
                    .ThenInclude(bbt => bbt.BinType)
                .Include(p => p.UploadedBy)
                    .ThenInclude(u => u.Role)
                        .ThenInclude(r => r.Permissions);

            queryWithIncludes = ApplySorting(queryWithIncludes, query.SortBy);

            var entityItems = await queryWithIncludes
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Loaded entities count: {Count}", entityItems.Count);

            try
            {
                var items = mapper.Map<List<BinPhoto>>(entityItems);
                _logger.LogInformation("Mapped items count: {Count}", items.Count);
                
                foreach (var item in items)
                {
                    _logger.LogInformation("PHOTO DEBUG: {Json}", System.Text.Json.JsonSerializer.Serialize(item));
                }

                return new PagedResult<BinPhoto>(items, totalCount, query.Page, query.PageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Mapping exception, dumping entityItems count: {Count}", entityItems.Count);

                foreach (var entity in entityItems)
                {
                    _logger.LogError("Entity ID: {Id}, BinPhotoBinTypes Count: {BinTypesCount}", 
                        entity.Id, entity.BinPhotoBinTypes?.Count ?? 0);
                }
                throw;
            }
        }

        // public async Task<Contracts.Models.PagedResult<BinPhoto>> GetUserPhotosAsync(
        //     Guid userId,
        //     PhotoQuery query,
        //     CancellationToken cancellationToken = default)
        // {
        //     var dbQuery = context.BinPhotos
        //         .AsNoTracking()
        //         .AsSplitQuery()                             // Разделение запросов для коллекций
        //         .Include(p => p.BinPhotoBinTypes)
        //             .ThenInclude(bbt => bbt.BinType)
        //         .Include(p => p.UploadedBy)
        //             .ThenInclude(u => u.Role)
        //                 .ThenInclude(r => r.Permissions)
        //         .Where(bp => bp.UploadedById == userId)
        //         .AsSplitQuery();
        //
        //     dbQuery = ApplyFilters(dbQuery, query);
        //     
        //     var totalCount = await dbQuery.CountAsync(cancellationToken);
        //     if (totalCount == 0)
        //     {
        //         return new Contracts.Models.PagedResult<BinPhoto>
        //         {
        //             Items = new List<BinPhoto>(),
        //             TotalCount = 0,
        //             Page = query.Page,
        //             PageSize = query.PageSize
        //         };
        //     }
        //     
        //     dbQuery = ApplySorting(dbQuery, query.SortBy);
        //     
        //     // Pagination
        //     var entityItems = await dbQuery
        //         .Skip((query.Page - 1) * query.PageSize)
        //         .Take(query.PageSize)
        //         .ToListAsync(cancellationToken);
        //     
        //     var items = mapper.Map<List<BinPhoto>>(entityItems);
        //     
        //     _logger.LogInformation(
        //         "Loaded {Count} photos for user {UserId}, page {Page}/{TotalPages}",
        //         items.Count, userId, query.Page, Math.Ceiling(totalCount / (double)query.PageSize));
        //
        //     return new PagedResult<BinPhoto>
        //     {
        //         Items = items,
        //         TotalCount = totalCount,
        //         Page = query.Page,
        //         PageSize = query.PageSize,
        //     };
        //     
        //     // var photos = await context.BinPhotos
        //     //     .Include(bp => bp.BinPhotoBinTypes)
        //     //         .ThenInclude(bbt => bbt.BinType)
        //     //     .Include(bp => bp.UploadedBy)
        //     //         .ThenInclude(u => u.Role)
        //     //         .ThenInclude(r => r.Permissions)
        //     //     .Where(bp => bp.UploadedBy.Id == userId)
        //     //     .ToListAsync();
        //     //
        //     // var binPhotos = mapper.Map<List<BinPhoto>>(photos);
        //     // return binPhotos;
        // }

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
        
        private static IQueryable<BinPhotoEntity> ApplyFilters(
            IQueryable<BinPhotoEntity> query,
            PhotoQuery filters)
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

        private static IQueryable<BinPhotoEntity> ApplySorting(
            IQueryable<BinPhotoEntity> query, 
            string sortBy)
        {
            return sortBy switch
            {
                "dateAsc" => query.OrderBy(bp => bp.UploadedAt),
                "dateDesc" => query.OrderByDescending(bp => bp.UploadedAt),
                "fillLevelAsc" => query.OrderBy(bp => bp.FillLevel)
                    .ThenByDescending(bp => bp.UploadedAt),
                "fillLevelDesc" => query.OrderByDescending(bp => bp.FillLevel)
                    .ThenByDescending(bp => bp.UploadedAt),
                "totalBinsDesc" => query.OrderByDescending(bp => bp.TotalBins)
                    .ThenByDescending(bp => bp.UploadedAt),
                _ => query.OrderByDescending(bp => bp.TotalBins)
            };
        }
    }
}
