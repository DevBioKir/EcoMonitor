using EcoMonitor.App.Services.Authorization;
using EcoMonitor.Contracts.Contracts;
using EcoMonitor.Contracts.Contracts.BinPhoto;
using EcoMonitor.Contracts.Contracts.BinPhotoUpload;
using EcoMonitor.Contracts.Models;
using EcoMonitor.Core.Models;
using EcoMonitor.Core.ValueObjects;
using EcoMonitor.DataAccess.Repositories;
using EcoMonitor.DataAccess.Repositories.Users;
using EcoMonitor.Infrastracture.Abstractions;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Logging;


namespace EcoMonitor.App.Services
{
    public class BinPhotoService(
        IMapper _mapper,
        IBinPhotoRepository _binPhotoRepository,
        IBinTypeRepository _binTypeRepository,
        ILogger<BinPhotoService> logger,
        IImagePipeline _pipeline,
        IUserRepository _userRepository,
        IAuthorizationService _authorizationService)
        : IBinPhotoService
    {
        private readonly ILogger<BinPhotoService> _logger = logger;

        public async Task<BinPhotoResponse> AddBinPhotoAsync(
            AddPhotoRequest request)
        {
            if (request == null)
                throw new KeyNotFoundException("Request is null");
            
            var uploadedBy = await _userRepository.GetByIdAsync(request.UploadedById);
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
            
            var addBinPhoto = await _binPhotoRepository.AddBinPhotoAsync(domainBinPhoto);

            return _mapper.Map<BinPhotoResponse>(addBinPhoto);
        }
        
        public async Task<Guid> DeleteBinPhotoAsync(Guid binPhotoId)
        {
            return await _binPhotoRepository.DeleteBinPhotoAsync(binPhotoId);
        }

        public async Task <IReadOnlyList<PhotoMarkerDTO>> GetMarkersAsync()
        {
            var markers = await _binPhotoRepository.GetMarkersAsync();
            return _mapper.Map<IReadOnlyList<PhotoMarkerDTO>>(markers);
        }

        public async Task<IReadOnlyList<BinPhotoResponse>> GetAllBinPhotosAsync()
        {
            var listBinPhotos = await _binPhotoRepository.GetAllBinPhotosAsync();
            return _mapper.Map<List<BinPhotoResponse>>(listBinPhotos);
        }
        
        public async Task<PagedResultDTO<BinPhotoResponse>> GetAllPhotosWithFilterAsync(
            PhotoQuery query,
            CancellationToken cancellationToken = default)
        {
            if (!query.IsValid(out var error))
            {
                throw new ArgumentException(error);
            }
            var result = await _binPhotoRepository.GetAllPhotosWithFilterAsync(query, cancellationToken);
            
            var items = _mapper.Map<List<BinPhotoResponse>>(result.Items);

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
        
        public async Task<PagedResultDTO<BinPhotoResponse>> GetUserPhotosAsync(
            Guid userId,
            PhotoQuery query,
            CancellationToken cancellationToken = default)
        {
            if (!query.IsValid(out var error))
            {
                throw new ArgumentException(error);
            }
            var result = await _binPhotoRepository.GetUserPhotosAsync(userId, query, cancellationToken);
            
            var items = _mapper.Map<List<BinPhotoResponse>>(result.Items);

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
            var domainBinPhoto = await _binPhotoRepository.GetPhotoByIdAsync(photoBinId);
            return _mapper.Map<BinPhotoResponse>(domainBinPhoto);
        }

        public async Task<IEnumerable<BinPhotoResponse>> GetPhotosInBoundsAsync(
            double north, 
            double south, 
            double east, 
            double west)
        {
            var photos = await _binPhotoRepository.GetPhotosInBoundsAsync(north, south, east, west);

            return _mapper.Map<IEnumerable<BinPhotoResponse>>(photos);
        }

        public async Task<BinPhotoResponse> UploadPhotoAsync(
            BinPhotoUploadRequest request, 
            Guid userId,
            CancellationToken ct = default)
        {
            if (request.Photo == null)
                throw new ArgumentNullException(nameof(request.Photo), "Photo is required");
            
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException($"User with ID {userId} not found");

            // var processed = await pipeline.ProcessAsync(request.Photo);
            // if (processed == null)
            //     throw new InvalidOperationException("Image processing failed");
            
            var binTypes = await _binTypeRepository.GetBinTypeByCodeAsync(request.BinTypeCode);
            if (binTypes == null || !binTypes.Any())
                throw new InvalidOperationException($"No bin types found for code {request.BinTypeCode}");

            var (lat, lon, originalUrl, fileName) = await UploadImageAsync(request.Photo, ct);

            // var lat = processed.Gps?.lat ?? 0;
            // var lon = processed.Gps?.lon ?? 0;
            // if (double.IsNaN(lat)) lat = 0;
            // if (double.IsNaN(lon)) lon = 0;

            var binPhoto = BinPhoto.Create(
                fileName: Path.GetFileName(request.Photo.FileName),
                urlFile: originalUrl ?? 
                         throw new InvalidOperationException("Processed image URL is null"),
                latitude: lat,
                longitude: lon,
                BinTypeId: binTypes.Select(bt => bt.Id).ToList(),
                fillLevel: request.FillLevel,
                isOutsideBin: request.IsOutsideBin,
                comment: request.Comment,
                totalBins: request.TotalBins,
                uploadedById: user.Id
            );
            // var binPhoto = BinPhoto.Create(
            //     fileName: Path.GetFileName(request.Photo.FileName),
            //     urlFile: processed.OriginalUrl ?? 
            //              throw new InvalidOperationException("Processed image URL is null"),
            //     latitude: lat,
            //     longitude: lon,
            //     BinTypeId: binTypes.Select(bt => bt.Id).ToList(),
            //     fillLevel: request.FillLevel,
            //     isOutsideBin: request.IsOutsideBin,
            //     comment: request.Comment,
            //     totalBins: request.TotalBins,
            //     uploadedById: user.Id
            //     );

            await _binPhotoRepository.AddBinPhotoAsync(binPhoto);

            return _mapper.Map<BinPhotoResponse>(binPhoto);
        }

        public async Task UpdatePhotoAsync(
            Guid actorId, 
            Guid photoId,
            UpdatePhotoRequest request,
            CancellationToken cancellationToken = default)
        {
            var actor = await _userRepository.GetByIdAsync(actorId, cancellationToken) 
                        ?? throw new UnauthorizedAccessException("Actor not found");
            
            _authorizationService.CheckPermisson(actor, Permission.PhotosEdit);
            
            var selectedPhoto = await _binPhotoRepository.GetPhotoByIdAsync(photoId, cancellationToken) 
                                ?? throw new KeyNotFoundException($"Photo with id {photoId} not found");
            
            _logger.LogInformation("UPDATE PHOTO: {Json}", System.Text.Json.JsonSerializer.Serialize(selectedPhoto));
            
            bool newPhotoFile = request.Photo != null;

            if (newPhotoFile)
            {
                var processed = await UploadImageAsync(request.Photo, cancellationToken);
                
                selectedPhoto.UpdateFile(
                    fileName: processed.FileName,
                    urlFile: processed.OriginalUrl,
                    latitude: processed.Latitude,
                    longitude: processed.Longitude);
            }
            
            selectedPhoto.UpdateMetadata(
                request.FillLevel ?? selectedPhoto.FillLevel, 
                request.IsOutsideBin ??  selectedPhoto.IsOutsideBin,
                request.Comment ?? selectedPhoto.Comment,
                request.TotalBins ?? selectedPhoto.TotalBins,
                request.BinTypeId ?? selectedPhoto.BinPhotoBinTypes.Select(x => x.BinTypeId));
            
            await _binPhotoRepository.UpdateBinPhotoAsync(selectedPhoto, cancellationToken);
        }

        private async Task<(double Latitude, double Longitude, string OriginalUrl, string FileName)> UploadImageAsync(
            IFormFile file, CancellationToken cancellationToken = default)
        {
            var processed = await _pipeline.ProcessAsync(file, cancellationToken);
            
            if (processed == null)
                throw new InvalidOperationException("Image processing failed");
            
            var lat = processed.Gps?.lat ?? 0;
            var lon = processed.Gps?.lon ?? 0;
            
            if (double.IsNaN(lat)) lat = 0;
            if (double.IsNaN(lon)) lon = 0;

            return ( 
                Latitude: lat, 
                Longitude: lon, 
                OriginalUrl: processed.OriginalUrl ?? throw new InvalidOperationException("Processed image URL is null"), 
                FileName: processed.FileName ?? throw new InvalidOperationException("Processed image FileName is null"));
        }
    }
    
    // public static class LoggingExtensions
    // {
    //     public static void LogDto<T>(this ILogger logger, T dto, [CallerMemberName] string? caller = null)
    //     {
    //         var json = System.Text.Json.JsonSerializer.Serialize(dto, new()
    //         {
    //             WriteIndented = true,
    //             PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    //         });
    //         logger.LogInformation("📦 {Caller} DTO: {Json}", caller, json);
    //     }
    // }
}
