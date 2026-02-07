using EcoMonitor.Contracts.Models;
using EcoMonitor.Core.Models;
using EcoMonitor.Core.Models.Users;
using EcoMonitor.DataAccess.Entities;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;

namespace EcoMonitor.DataAccess.Repositories
{
    public class BinPhotoRepository(
        EcoMonitorDbContext _context,
        IMapper mapper,
        ILogger<BinPhotoRepository> _logger) : IBinPhotoRepository
    {
        public async Task<IReadOnlyList<PhotoMarker>> GetMarkersAsync(CancellationToken cancellationToken = default)
        {
            var allPhotos = await _context.BinPhotos
                .Select(p => new { p.Id, p.Location, p.UrlFile, p.UploadedAt })
                .ToListAsync(cancellationToken);

            var lastPhotos = allPhotos
                .GroupBy(p => new
                {
                    Lat = Math.Round(p.Location.Y, 3),
                    Lng = Math.Round(p.Location.X, 3)
                })
                .Select(g => g.OrderByDescending(p => p.UploadedAt).First())
                .Select(p => new PhotoMarker
                {
                    Id = p.Id,
                    Latitude = p.Location.Y,
                    Longitude = p.Location.X,
                    PhotoUrl = p.UrlFile,
                })
                .ToList();

            return lastPhotos;
            // const string sql = @"
            //     SELECT DISTINCT ON (
            //         round(ST_Y(ST_Force2D(""Location""::geometry))::numeric, 5),
            //         round(ST_X(ST_Force2D(""Location""::geometry))::numeric, 5)
            //     )
            //         ""Id"",
            //         ST_Y(ST_Force2D(""Location""::geometry)) AS ""Latitude"",
            //         ST_X(ST_Force2D(""Location""::geometry)) AS ""Longitude"",
            //         ""UrlFile"" AS ""PhotoUrl"",
            //         ""UploadedAt""
            //     FROM ""BinPhotos""
            //     ORDER BY 
            //         round(ST_Y(ST_Force2D(""Location""::geometry))::numeric, 5),
            //         round(ST_X(ST_Force2D(""Location""::geometry))::numeric, 5),
            //         ""UploadedAt"" DESC;
            // ";
            //
            // return await _context.Set<PhotoMarker>()
            //     .FromSqlRaw(sql)
            //     .ToListAsync(cancellationToken);
        }
        
        // public async Task<IReadOnlyList<PhotoMarker>> GetMarkersAsync(CancellationToken cancellationToken = default)
        // {
        //     const string sql = @"
        //     SELECT 
        //         ""Id"", 
        //         ST_Y(ST_Force2D(""Location""::geometry)) AS ""Latitude"", 
        //         ST_X(ST_Force2D(""Location""::geometry)) AS ""Longitude"", 
        //         ""UrlFile"" AS ""PhotoUrl""
        //     FROM ""BinPhotos""
        // ";
        //
        //     return await _context.Set<PhotoMarker>()
        //         .FromSqlRaw(sql)
        //         .ToListAsync(cancellationToken);
        //     // return await context.BinPhotos
        //     //     .Select(bp => new PhotoMarker
        //     //     {
        //     //         Id = bp.Id,
        //     //         Latitude = EF.Functions.ST_Y(EF.Functions.ST_Force2D(bp.Location)),
        //     //         Longitude = EF.Functions.ST_X(EF.Functions.ST_Force2D(bp.Location)),
        //     //         // Latitude = bp.Location.Y,
        //     //         // Longitude = bp.Location.X,
        //     //         PhotoUrl = bp.UrlFile
        //     //     })
        //     //     .ToListAsync();
        // }

        public async Task<IReadOnlyList<BinPhoto>> GetAllBinPhotosAsync()
        {
            var binPhotosEntity = await _context.BinPhotos
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

            return mapper.Map<List<BinPhoto>>(photos);
        }

        // public async Task<BinPhoto> GetByCoordinatesAsync(
        //     double latitude, 
        //     double longitude, 
        //     CancellationToken cancellationToken = default)
        // {
        //     // var point = NetTopologySuite.Geometries.GeometryFactory.Default.CreatePoint(new Coordinate(latitude, longitude));
        //     //
        //     // point.SRID = 4326;
        //     //
        //     //
        //     // var query = await _context.BinPhotos
        //     //     .AsNoTracking()
        //     //     .Where(p => 
        //     //         EF.Functions.Distance(
        //     //             p.Location,
        //     //             point) <= 30)
        //     //     .OrderBy(p => EF.Functions.Distance(p.Location, point))
        //     //     .FirstOrDefaultAsync(cancellationToken); 
        //     //
        //     // return mapper.Map<BinPhoto>(query);
        // }

        public async Task<PagedResult<BinPhoto>> GetAllPhotosWithFilterAsync(
            PhotoQuery query,
            CancellationToken cancellationToken = default)
        {
            var baseQuery = _context.BinPhotos
                .AsNoTracking();

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

                // foreach (var item in items)
                // {
                //     _logger.LogInformation("PHOTO DEBUG: {Json}", System.Text.Json.JsonSerializer.Serialize(item));
                // }

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

        public async Task<PagedResult<BinPhoto>> GetUserPhotosAsync(
            Guid userId,
            PhotoQuery query,
            CancellationToken cancellationToken = default)
        {
            var baseQuery = _context.BinPhotos
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

                // foreach (var item in items)
                // {
                //     _logger.LogInformation("PHOTO DEBUG: {Json}", System.Text.Json.JsonSerializer.Serialize(item));
                // }

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

        public async Task<int> GetCountPhotosAsync(CancellationToken cancellationToken = default)
        {
            return await _context.BinPhotos.CountAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<BinPhoto>> GetLatestPhotosAsync(
            int count = 5,
            CancellationToken cancellationToken = default)
        {
            var entities = await _context.BinPhotos
                .AsNoTracking()
                .OrderByDescending(p => p.UploadedAt)
                .Include(p => p.BinPhotoBinTypes)
                .ThenInclude(bbt => bbt.BinType)
                .Include(p => p.UploadedBy)
                .ThenInclude(u => u.Role)
                .Take(count)
                .ToListAsync(cancellationToken);
            //.ThenInclude(u => u.Role);

            return mapper.Map<List<BinPhoto>>(entities);
        }

        public async Task<BinPhoto> AddBinPhotoAsync(
            BinPhoto binPhoto)
        {
            var binPhotoEntity = mapper.Map<BinPhotoEntity>(binPhoto);

            await _context.BinPhotos.AddAsync(binPhotoEntity);
            await _context.SaveChangesAsync();
            return binPhoto;
        }

        public async Task UpdateBinPhotoAsync(BinPhoto binPhoto, CancellationToken cancellationToken = default)
        {
            try
            {
                var entityBinPhoto = await _context.BinPhotos
                    // .Include(bp => bp.BinPhotoBinTypes)
                    // .ThenInclude(bbt => bbt.BinType)
                    // .Include(bp => bp.UploadedBy)
                    // .ThenInclude(u => u.Role)
                    // .ThenInclude(r => r.Permissions)
                    .FirstOrDefaultAsync(b => b.Id == binPhoto.Id);

                if (entityBinPhoto is null)
                    throw new KeyNotFoundException();
                
                if (!string.IsNullOrWhiteSpace(binPhoto.FileName))
                {
                    entityBinPhoto.FileName = binPhoto.FileName;
                    entityBinPhoto.UrlFile = binPhoto.UrlFile;
                    entityBinPhoto.Location = new Point(
                        binPhoto.Longitude,
                        binPhoto.Latitude) { SRID = 4326 };
                }
                
                entityBinPhoto.District = binPhoto.District;
                entityBinPhoto.FillLevel = binPhoto.FillLevel;
                entityBinPhoto.Comment = binPhoto.Comment;
                entityBinPhoto.TotalBins = binPhoto.TotalBins;
                entityBinPhoto.IsOutsideBin = binPhoto.IsOutsideBin;

                _context.BinPhotoBinType.RemoveRange(entityBinPhoto.BinPhotoBinTypes);

                foreach (var binPhotoBinType in binPhoto.BinPhotoBinTypes)
                {
                    _context.BinPhotoBinType.Add(new BinPhotoBinTypeEntity
                    {
                        BinPhotoId = entityBinPhoto.Id,
                        BinTypeId = binPhotoBinType.BinTypeId
                    });
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении фото {PhotoId}", binPhoto.Id);
                throw;
            }
        }

        public async Task<BinPhoto> GetPhotoByIdAsync(Guid photoBinId, CancellationToken cancellationToken = default)
        {
            var entityBinPhoto = await _context.BinPhotos
                .Include(bp => bp.BinPhotoBinTypes)
                .ThenInclude(bbt => bbt.BinType)
                .Include(bp => bp.UploadedBy)
                .ThenInclude(u => u.Role)
                .ThenInclude(r => r.Permissions)
                .FirstOrDefaultAsync(b => b.Id == photoBinId, cancellationToken);

            if (entityBinPhoto == null)
                throw new NullReferenceException($"Container photo with ID {photoBinId} not found");

            return mapper.Map<BinPhoto>(entityBinPhoto);
        }

        public async Task<Guid> DeleteBinPhotoAsync(Guid binPhotoId)
        {
            var binPhotoEntity = await _context.BinPhotos
                .FirstOrDefaultAsync(b => b.Id == binPhotoId);

            if (binPhotoEntity == null)
                throw new NullReferenceException($"Container photo with ID {binPhotoId} not found");

            _context.BinPhotoBinType.RemoveRange(binPhotoEntity.BinPhotoBinTypes);

            _context.BinPhotos.Remove(binPhotoEntity);
            await _context.SaveChangesAsync();

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
            if (filters.OnlyOutsideBin.HasValue)
            {
                query = query.Where(p => p.IsOutsideBin == filters.OnlyOutsideBin.Value);
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

            foreach (var filteredDate in query)
            {
                Console.WriteLine(filteredDate);
            }

            if (!string.IsNullOrWhiteSpace(filters.UploadedBySearch))
            {
                var term = filters.UploadedBySearch.ToLower().Trim();

                query = query.Where(u => u.UploadedBy.Email.ToLower().Contains(term)
                                         || u.UploadedBy.Firstname.ToLower().Contains(term)
                                         || u.UploadedBy.Surname.ToLower().Contains(term));
            }

            if (filters.District.HasValue && filters.District.Value != 0)
            {
                var districtEnum = (District)filters.District.Value;
                query = query.Where(u => u.District == districtEnum);
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