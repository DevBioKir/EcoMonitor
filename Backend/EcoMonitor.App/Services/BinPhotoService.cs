using EcoMonitor.Contracts.Contracts;
using EcoMonitor.Core.Models;
using EcoMonitor.DataAccess.Repositories;
using MapsterMapper;
using Microsoft.Extensions.Logging;
using System.Reflection.Metadata.Ecma335;

namespace EcoMonitor.App.Services
{
    public class BinPhotoService : IBinPhotoService
    {
        private readonly IMapper _mapper;
        private readonly IBinPhotoRepository _binPhotoRepository;
        private readonly ILogger<BinPhotoService> _logger;
        public BinPhotoService(
            IMapper mapper,
            IBinPhotoRepository binPhotoRepository,
            ILogger<BinPhotoService> logger)
        {
            _mapper = mapper;
            _binPhotoRepository = binPhotoRepository;
            _logger = logger;
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
    }
}
