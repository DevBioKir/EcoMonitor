using EcoMonitor.App.Services;
using EcoMonitor.Contracts.Contracts.BinPhoto;
using EcoMonitor.Contracts.Contracts.BinPhotoUpload;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace EcoMonitor.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BinPhotoController : ControllerBase
    {
        private readonly IBinPhotoService _binPhotoService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<BinPhotoController> _logger;

        public BinPhotoController(
            IBinPhotoService binPhotoService,
            IMapper mapper,
            IWebHostEnvironment env,
            ILogger<BinPhotoController> logger)
        {
            _binPhotoService = binPhotoService;
            _mapper = mapper;
            _env = env;
            _logger = logger;
        }

        [HttpGet("GetBinPhotoById/{id}")]
        public async Task<ActionResult<BinPhotoResponse>> GetBinPhotoByIdAsync(Guid id)
        {
            try
            {
                var binPhoto = await _binPhotoService.GetPhotoByIdAsync(id);
                return Ok(binPhoto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при поиске фото в базе");
                return StatusCode(500, "Ошибка при поиске фото в базе.");
            }
        }

        [HttpGet("GetPhotosInBounds")]
        public async Task<ActionResult<IEnumerable<BinPhotoResponse>>> GetPhotosInBoundsAsync(
            double north,
            double south,
            double east,
            double west)
        {
            try
            {
                var photos = await _binPhotoService.GetPhotosInBoundsAsync(north, south, east, west);
                return Ok(photos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при поиске фотографий в базе");
                return StatusCode(500, "Ошибка при поиске фотографий в базе.");
            }
        }

        [HttpGet("GetAllPhotos")]
        public async Task<ActionResult<IReadOnlyList<BinPhotoResponse>>> GetAllBinPhotosAsync()
        {
            var binPhotos = await _binPhotoService.GetAllBinPhotosAsync();
            var responseBinPhotos = _mapper.Map<List<BinPhotoResponse>>(binPhotos);

            return Ok(responseBinPhotos);
        }

        [HttpPost]
        public async Task<ActionResult<BinPhotoResponse>> AddBinPhotoAsync([FromBody] BinPhotoRequest request)
        {
            var binPhoto = await _binPhotoService.AddBinPhotoAsync(request);

            return CreatedAtAction(nameof(AddBinPhotoAsync), new { id = binPhoto.Id }, binPhoto);
        }

        [HttpPost("UploadWithMetadata")]
        public async Task<ActionResult<BinPhotoResponse>> UploadWithMetadata([FromForm] BinPhotoUploadRequest request, CancellationToken ct)
        {
            _logger.LogInformation("UploadWithMetadata вызван");

            if (request == null)
            {
                _logger.LogWarning("Request model пустая (null)");
                return BadRequest("Данные не переданы");
            }

            if (request.Photo == null)
            {
                _logger.LogWarning("Фото не передано");
                return BadRequest("Фото не загружено");
            }

            _logger.LogInformation("Получены данные: BinType={BinTypeId}, FillLevel={FillLevel}, IsOutsideBin={IsOutsideBin}, Comment={Comment}",
                request.BinTypeId, request.FillLevel, request.IsOutsideBin, request.Comment);

            _logger.LogInformation("Фото: FileName={FileName}, ContentType={ContentType}, Length={Length}",
                request.Photo.FileName, request.Photo.ContentType, request.Photo.Length);

            try
            {
                var binPhoto = await _binPhotoService.UploadImage(request, ct);
                _logger.LogInformation("Фото успешно загружено, Id={Id}", binPhoto.Id);

                return Ok(binPhoto);
                //return CreatedAtAction(nameof(AddBinPhotoAsync), new { id = binPhoto.Id }, binPhoto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке фото с метаданными");
                return StatusCode(500, "Ошибка при обработке изображения.");
            }
        }

        [HttpDelete("Delete")]
        public async Task<ActionResult<Guid>> DeleteBinPhotoAsync(Guid binPhotoId)
        {
            try
            {
                var photoId = await _binPhotoService.DeleteBinPhotoAsync(binPhotoId);
                return photoId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении фото из базы");
                return StatusCode(500, "Ошибка при удалении фотографии.");
            }
        }
    }
}
