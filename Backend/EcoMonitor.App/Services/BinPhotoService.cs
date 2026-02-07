using System.ComponentModel.DataAnnotations;
using System.Globalization;
using EcoMonitor.App.Services.Authorization;
using EcoMonitor.Contracts.Contracts;
using EcoMonitor.Contracts.Contracts.BinPhoto;
using EcoMonitor.Contracts.Contracts.BinPhotoUpload;
using EcoMonitor.Contracts.Models;
using EcoMonitor.Core.Models;
using EcoMonitor.Core.Models.Users;
using EcoMonitor.Core.ValueObjects;
using EcoMonitor.DataAccess.Repositories;
using EcoMonitor.DataAccess.Repositories.Users;
using EcoMonitor.Infrastracture.Abstractions;
using MapsterMapper;
using Microsoft.AspNetCore.Hosting;
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
        IAuthorizationService _authorizationService,
        IWebHostEnvironment _environment)
        : IBinPhotoService
    {
        private readonly ILogger<BinPhotoService> _logger = logger;

        public async Task<int> GetCountPhotosAsync(CancellationToken cancellationToken = default)
        {
            return await _binPhotoRepository.GetCountPhotosAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<BinPhotoResponse>> GetLatestPhotosAsync(
            int count = 5,
            CancellationToken cancellationToken = default)
        {
            var latestPhotos = await _binPhotoRepository.GetLatestPhotosAsync(count, cancellationToken);
            return _mapper.Map<IReadOnlyList<BinPhotoResponse>>(latestPhotos);
        }

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
                (District)request.District,
                request.BinTypeId,
                request.FillLevel,
                request.IsOutsideBin,
                request.Comment,
                request.TotalBins,
                uploadedBy.Id);
            
            var addBinPhoto = await _binPhotoRepository.AddBinPhotoAsync(domainBinPhoto);

            return _mapper.Map<BinPhotoResponse>(addBinPhoto);
        }
        
        public async Task DeleteBinPhotoAsync(Guid binPhotoId)
        {
            var photo = await _binPhotoRepository.GetPhotoByIdAsync(binPhotoId);
            
            if (!string.IsNullOrWhiteSpace(photo.UrlFile))
            {
                DeleteFileSafe(photo.UrlFile);
            }
            
            await _binPhotoRepository.DeleteBinPhotoAsync(binPhotoId);
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

        // public async Task<BinPhotoMapResponse> GetByCoordinatesAsync(
        //     double latitude, 
        //     double longitude, 
        //     CancellationToken cancellationToken = default)
        // {
        //     var photosByCoordinates = await _binPhotoRepository.GetByCoordinatesAsync(latitude, longitude, cancellationToken);
        //     
        //     return _mapper.Map<BinPhotoMapResponse>(photosByCoordinates);
        // }

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
                district: (District)request.District,
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
            _logger.LogWarning("=== UPDATE PHOTO START ===");
            _logger.LogWarning("PhotoId: {PhotoId}", photoId);
            _logger.LogWarning("New photo uploaded: {HasPhoto}", request.Photo != null);
            
            double? parsedFillLevel = null;
            
            if (!string.IsNullOrWhiteSpace(request.FillLevel))
            {
                if (!double.TryParse(
                        request.FillLevel, 
                        NumberStyles.Float, 
                        CultureInfo.InvariantCulture, 
                        out var value))
                {
                    throw new ValidationException("FillLevel has invalid format. Use 0.0–1.0");
                }

                if (value < 0.0 || value > 1.0)
                {
                    throw new ValidationException("FillLevel must be between 0.0 and 1.0");
                }

                parsedFillLevel = value;
            }
            
            var actor = await _userRepository.GetByIdAsync(actorId, cancellationToken) 
                        ?? throw new UnauthorizedAccessException("Actor not found");
            
            _authorizationService.CheckPermisson(actor, Permission.PhotosEdit);
            
            var selectedPhoto = await _binPhotoRepository.GetPhotoByIdAsync(photoId, cancellationToken) 
                                ?? throw new KeyNotFoundException($"Photo with id {photoId} not found");
            
            _logger.LogWarning(
                "CURRENT PHOTO UrlFile: '{UrlFile}'",
                selectedPhoto.UrlFile
            );
            
            _logger.LogInformation("UPDATE PHOTO: {Json}", System.Text.Json.JsonSerializer.Serialize(selectedPhoto));
            
            bool newPhotoFile = request.Photo != null;

            if (newPhotoFile)
            {
                _logger.LogWarning("Uploading new photo...");
                
                var processed = await UploadImageAsync(request.Photo, cancellationToken);
                
                _logger.LogWarning(
                    "New photo uploaded. New UrlFile: '{NewUrl}'",
                    processed.OriginalUrl
                );

                string? oldPhotoPath = null;

                if (!string.IsNullOrWhiteSpace(selectedPhoto.UrlFile))
                {
                    oldPhotoPath = Path.Combine(
                        _environment.WebRootPath,
                        selectedPhoto.UrlFile);
                    
                    _logger.LogWarning(
                        "Calculated old photo path: {OldPath}",
                        oldPhotoPath
                    );
                    
                    _logger.LogWarning(
                        "Old file exists: {Exists}",
                        File.Exists(oldPhotoPath)
                    );
                }
                else
                {
                    _logger.LogWarning("Old UrlFile is NULL or EMPTY");
                }
                
                selectedPhoto.UpdateFile(
                    fileName: processed.FileName,
                    urlFile: processed.OriginalUrl,
                    latitude: processed.Latitude,
                    longitude: processed.Longitude);

                if (oldPhotoPath != null)
                {
                    try
                    {
                        if (File.Exists(oldPhotoPath))
                        {
                            File.Delete(oldPhotoPath);
                            _logger.LogWarning("OLD PHOTO DELETED SUCCESSFULLY");
                        }
                        else
                        {
                            _logger.LogWarning("OLD PHOTO NOT FOUND ON DISK");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "ERROR WHILE DELETING OLD PHOTO: {Path}",
                            oldPhotoPath
                        );
                    }
                }
                
                // if (oldPhotoPath != null && File.Exists(oldPhotoPath))
                // {
                //     File.Delete(oldPhotoPath);
                // }
            }
            else
            {
                _logger.LogWarning("No new photo uploaded → skipping file deletion");
            }
            
            selectedPhoto.UpdateMetadata(
                request.District.HasValue ? (District)request.District.Value : selectedPhoto.District,
                parsedFillLevel ?? selectedPhoto.FillLevel, 
                request.IsOutsideBin ??  selectedPhoto.IsOutsideBin,
                request.Comment ?? selectedPhoto.Comment,
                request.TotalBins ?? selectedPhoto.TotalBins);

            if (request.BinTypeId is not null)
            {
                selectedPhoto.BinPhotoBinTypes.Clear();

                foreach (var id in request.BinTypeId)
                {
                    selectedPhoto.BinPhotoBinTypes.Add(new BinPhotoBinType(selectedPhoto.Id, id));
                }
                
                //selectedPhoto.UpdateBinTypes(request.BinTypeId);
            }
            
            await _binPhotoRepository.UpdateBinPhotoAsync(selectedPhoto, cancellationToken);
            
            _logger.LogWarning("=== UPDATE PHOTO END ===");
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
        
        private void DeleteFileSafe(string filePath)
        {
            try
            {
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath);

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Не удалось удалить файл {Path}", filePath);
            }
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
