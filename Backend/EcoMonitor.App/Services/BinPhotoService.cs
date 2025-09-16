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
    public class BinPhotoService : IBinPhotoService
    {
        private readonly IMapper _mapper;
        private readonly IBinPhotoRepository _binPhotoRepository;
        private readonly ILogger<BinPhotoService> _logger;
        private readonly IImagePipeline _pipeline;
        private readonly IUserRepository _userRepository;

        //private readonly IImageStorageService _storage;
        //private readonly IGeolocationService _geo;

        public BinPhotoService(
            IMapper mapper,
            IBinPhotoRepository binPhotoRepository,
            ILogger<BinPhotoService> logger,
            IImagePipeline pipeline)
        //IImageStorageService storage,
        //IGeolocationService geo)
        {
            _mapper = mapper;
            _binPhotoRepository = binPhotoRepository;
            _logger = logger;
            _pipeline = pipeline;
            //_storage = storage;
            //_geo = geo;
        }
        public async Task<BinPhotoResponse> AddBinPhotoAsync(
            BinPhotoRequest requestBinPhoto)
        {
            var domainBinPhoto = _mapper.Map<BinPhoto>(requestBinPhoto);
            var addBinPhoto = await _binPhotoRepository.AddBinPhotoAsync(domainBinPhoto);

            return _mapper.Map<BinPhotoResponse>(addBinPhoto);
        }

        public async Task<Guid> DeleteBinPhotoAsync(Guid binPhotoId)
        {
            return await _binPhotoRepository.DeleteBinPhotoAsync(binPhotoId);
        }

        public async Task<IReadOnlyList<BinPhotoResponse>> GetAllBinPhotosAsync()
        {
            var listBinPhotos = await _binPhotoRepository.GetAllBinPhotosAsync();
            return _mapper.Map<List<BinPhotoResponse>>(listBinPhotos);
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

        public async Task<BinPhotoResponse> UploadImage(BinPhotoUploadRequest request, CancellationToken ct)
        {
            var user = await _userRepository.GetByIdAsync(request.UploadedById);

            var processed = await _pipeline.ProcessAsync(request.Photo);

            var binPhoto = BinPhoto.Create(
                fileName: Path.GetFileName(request.Photo.FileName),
                urlFile: processed.OriginalUrl,
                latitude: processed.Gps?.lat ?? 0,
                longitude: processed.Gps?.lon ?? 0,
                BinTypeId: request.BinTypeId,
                fillLevel: request.FillLevel,
                isOutsideBin: request.IsOutsideBin,
                comment: request.Comment,
                uploadedBy: user
                );

            await _binPhotoRepository.AddBinPhotoAsync(binPhoto);

            return _mapper.Map<BinPhotoResponse>(binPhoto);
        }
    }
}
