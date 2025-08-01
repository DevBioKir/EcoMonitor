using EcoMonitor.Contracts.Contracts;
using EcoMonitor.Core.Models;
using EcoMonitor.DataAccess.Repositories;
using EcoMonitor.Infrastracture.Abstractions;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;


namespace EcoMonitor.App.Services
{
    public class BinPhotoService : IBinPhotoService
    {
        private readonly IMapper _mapper;
        private readonly IBinPhotoRepository _binPhotoRepository;
        private readonly ILogger<BinPhotoService> _logger;

        private readonly IImageStorageService _storage;
        private readonly IGeolocationService _geo;

        public BinPhotoService(
            IMapper mapper,
            IBinPhotoRepository binPhotoRepository,
            ILogger<BinPhotoService> logger,
            IImageStorageService storage,
            IGeolocationService geo)
        {
            _mapper = mapper;
            _binPhotoRepository = binPhotoRepository;
            _logger = logger;
            _storage = storage;
            _geo = geo;
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

        public async Task<ICollection<BinPhotoResponse>> GetAllBinPhotosAsync()
        {
            var listBinPhotos = await _binPhotoRepository.GetAllBinPhotosAsync();
            return _mapper.Map<List<BinPhotoResponse>>(listBinPhotos);
        }

        public async Task<BinPhotoResponse> GetPhotoByIdAsync(Guid photoBinId)
        {
            var domainBinPhoto = await _binPhotoRepository.GetPhotoByIdAsync(photoBinId);
            return _mapper.Map<BinPhotoResponse>(domainBinPhoto);
        }

        public async Task<BinPhotoResponse> UploadImage(BinPhotoUploadRequest request)
        {
            var relativePath = await _storage.SaveImageAsync(request.Photo);
            var absolutePath = Path.Combine("wwwroot", relativePath);

            using var uploadedPhoto = await Image.LoadAsync(absolutePath);

            var exif = uploadedPhoto.Metadata.ExifProfile;

            _logger.LogInformation("Metadata: {Meta}", uploadedPhoto.Metadata);
            _logger.LogInformation("ExifProfile: {Exif}", uploadedPhoto.Metadata.ExifProfile);

            if (exif == null)
            {
                _logger.LogWarning("EXIF не найден");
                Console.WriteLine("EXIF не найден");
            }
            else
            {
                foreach (var val in exif.Values)
                {
                    _logger.LogInformation("EXIF: {Tag} = {Value}", val.Tag, val.GetValue());
                    Console.WriteLine($"EXIF: {val.Tag} = {val.GetValue()}");
                }
            }

            var (lat, lon) = _geo.GeoLocationService(exif);

            var binPhoto = BinPhoto.Create(
                fileName: Path.GetFileName(request.Photo.FileName),
                urlFile: "/" + relativePath.Replace("\\", "/"),
                latitude: lat ?? 0,
                longitude: lon ?? 0,
                BinTypeId: request.BinTypeId,
                fillLevel: request.FillLevel,
                isOutsideBin: request.IsOutsideBin,
                comment: request.Comment);

            await _binPhotoRepository.AddBinPhotoAsync(binPhoto);

            return _mapper.Map<BinPhotoResponse>(binPhoto);
        }
    }
}
