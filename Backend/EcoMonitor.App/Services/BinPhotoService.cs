using EcoMonitor.Contracts.Contracts;
using EcoMonitor.Contracts.Contracts.BinPhoto;
using EcoMonitor.Contracts.Contracts.BinPhotoUpload;
using EcoMonitor.Contracts.Models;
using EcoMonitor.Core.Models;
using EcoMonitor.DataAccess.Repositories;
using EcoMonitor.DataAccess.Repositories.Users;
using EcoMonitor.Infrastracture.Abstractions;
using MapsterMapper;
using Microsoft.Extensions.Logging;


namespace EcoMonitor.App.Services
{
    public class BinPhotoService(
        IMapper mapper,
        IBinPhotoRepository binPhotoRepository,
        IBinTypeRepository binTypeRepository,
        ILogger<BinPhotoService> logger,
        IImagePipeline pipeline,
        IUserRepository userRepository)
        : IBinPhotoService
    {
        private readonly ILogger<BinPhotoService> _logger = logger;

        public async Task<BinPhotoResponse> AddBinPhotoAsync(
            BinPhotoRequest request)
        {
            if (request == null)
                throw new KeyNotFoundException("Request is null");
            
            var uploadedBy = await userRepository.GetByIdAsync(request.UploadedById);
            if (uploadedBy == null)
                throw new KeyNotFoundException("User not found");
            
            var domainBinPhoto = BinPhoto.Create(
                request.FileName,
                request.UrlFile,
                request.Latitude,
                request.Longitude,
                request.BinTypeId,
                request.FillLevel,
                request.IsOutsideBin,
                request.Comment,
                request.TotalBins,
                uploadedBy.Id);
            
            var addBinPhoto = await binPhotoRepository.AddBinPhotoAsync(domainBinPhoto);

            return mapper.Map<BinPhotoResponse>(addBinPhoto);
        }
        
        public async Task<Guid> DeleteBinPhotoAsync(Guid binPhotoId)
        {
            return await binPhotoRepository.DeleteBinPhotoAsync(binPhotoId);
        }

        public async Task <IReadOnlyList<PhotoMarkerDTO>> GetMarkersAsync()
        {
            var markers = await binPhotoRepository.GetMarkersAsync();
            return mapper.Map<IReadOnlyList<PhotoMarkerDTO>>(markers);
        }

        public async Task<IReadOnlyList<BinPhotoResponse>> GetAllBinPhotosAsync()
        {
            var listBinPhotos = await binPhotoRepository.GetAllBinPhotosAsync();
            return mapper.Map<List<BinPhotoResponse>>(listBinPhotos);
        }
        
        public async Task<PagedResultDTO<BinPhotoResponse>> GetUserPhotosAsync(
            Guid userId,
            PhotoQuery query,
            CancellationToken cancellationToken = default)
        {
            if (!query.IsValid(out var error))
            {
                throw new ArgumentException(error);
            }
            var result = await binPhotoRepository.GetUserPhotosAsync(userId, query, cancellationToken);
            
            var items = mapper.Map<List<BinPhotoResponse>>(result.Items);

            foreach (var item in items)
            {
                _logger.LogInformation("PHOTO DEBUG: {Json}", System.Text.Json.JsonSerializer.Serialize(item));
            }
            
            return new PagedResultDTO<BinPhotoResponse>
            {
                Items = items,
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
            // var result = await userRepository.GetByIdAsync(userId);
            // return mapper.Map<List<BinPhotoResponse>>(listBinPhotos);
        }

        public async Task<BinPhotoResponse> GetPhotoByIdAsync(Guid photoBinId)
        {
            var domainBinPhoto = await binPhotoRepository.GetPhotoByIdAsync(photoBinId);
            return mapper.Map<BinPhotoResponse>(domainBinPhoto);
        }

        public async Task<IEnumerable<BinPhotoResponse>> GetPhotosInBoundsAsync(
            double north, 
            double south, 
            double east, 
            double west)
        {
            var photos = await binPhotoRepository.GetPhotosInBoundsAsync(north, south, east, west);

            return mapper.Map<IEnumerable<BinPhotoResponse>>(photos);
        }

        public async Task<BinPhotoResponse> UploadImage(
            BinPhotoUploadRequest request, 
            Guid userId,
            CancellationToken ct = default)
        {
            if (request.Photo == null)
                throw new ArgumentNullException(nameof(request.Photo), "Photo is required");
            
            var user = await userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException($"User with ID {userId} not found");

            var processed = await pipeline.ProcessAsync(request.Photo);
            if (processed == null)
                throw new InvalidOperationException("Image processing failed");
            
            var binTypes = await binTypeRepository.GetBinTypeByCodeAsync(request.BinTypeCode);
            if (binTypes == null || !binTypes.Any())
                throw new InvalidOperationException($"No bin types found for code {request.BinTypeCode}");

            var binPhoto = BinPhoto.Create(
                fileName: Path.GetFileName(request.Photo.FileName),
                urlFile: processed.OriginalUrl ?? 
                         throw new InvalidOperationException("Processed image URL is null"),
                latitude: processed.Gps?.lat ?? 0,
                longitude: processed.Gps?.lon ?? 0,
                BinTypeId: binTypes.Select(bt => bt.Id).ToList(),
                fillLevel: request.FillLevel,
                isOutsideBin: request.IsOutsideBin,
                comment: request.Comment,
                totalBins: request.TotalBins,
                uploadedById: user.Id
                );

            await binPhotoRepository.AddBinPhotoAsync(binPhoto);

            return mapper.Map<BinPhotoResponse>(binPhoto);
        }
    }
}
