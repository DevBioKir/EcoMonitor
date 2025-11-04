using EcoMonitor.App.Models.Queries;
using EcoMonitor.Contracts.Contracts;
using EcoMonitor.Contracts.Contracts.BinPhoto;
using EcoMonitor.Contracts.Contracts.BinPhotoUpload;
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

        public async Task<IReadOnlyList<BinPhotoResponse>> GetAllBinPhotosAsync()
        {
            var listBinPhotos = await binPhotoRepository.GetAllBinPhotosAsync();
            return mapper.Map<List<BinPhotoResponse>>(listBinPhotos);
        }
        
        public async Task<PagedResult<BinPhotoResponse>> GetUserPhotosAsync(
            Guid userId,
            PhotoQuery query,
            CancellationToken cancellationToken = default)
        {
            var query = await userRepository.GetByIdAsync(userId);
            return mapper.Map<List<BinPhotoResponse>>(listBinPhotos);
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

        public async Task<BinPhotoResponse> UploadImage(BinPhotoUploadRequest request, CancellationToken ct)
        {
            var user = await userRepository.GetByIdAsync(request.UploadedById);

            var processed = await pipeline.ProcessAsync(request.Photo);
            
            var binTypes = await binTypeRepository.GetBinTypeByCodeAsync(request.BinTypeCode);

            var binPhoto = BinPhoto.Create(
                fileName: Path.GetFileName(request.Photo.FileName),
                urlFile: processed.OriginalUrl,
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
