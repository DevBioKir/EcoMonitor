using System.Security.Claims;
using EcoMonitor.API.Attributes;
using EcoMonitor.App.Services;
using EcoMonitor.Contracts.Contracts;
using EcoMonitor.Contracts.Contracts.BinPhoto;
using EcoMonitor.Contracts.Contracts.BinPhotoUpload;
using EcoMonitor.Contracts.Models;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoMonitor.API.Controllers
{
    [PublicApi]
    [Authorize]
    [Route("[controller]")]
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

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) throw new UnauthorizedAccessException("User is not authenticated");
            return Guid.Parse(userIdClaim.Value);
        }
        
        [HttpGet("GetBinPhotoById")]
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
        
        [HttpGet("allPhotos")]
        public async Task<ActionResult<PagedResultDTO<BinPhotoResponse>>> GetAllPhotosWithFilterAsync(
            [FromQuery] PhotoFilterDTO filterDto,
            CancellationToken cancellationToken)
        {
            try
            {
                //var user = GetCurrentUserId();
                
                var query = _mapper.Map<PhotoQuery>(filterDto);

                var photos = await _binPhotoService.GetAllPhotosWithFilterAsync(
                    query,
                    cancellationToken);

                return Ok(photos);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
        
        [HttpGet("userUploadedPhotos")]
        public async Task<ActionResult<PagedResultDTO<BinPhotoResponse>>> GetUserPhotosAsync(
            [FromQuery] PhotoFilterDTO filterDto,
            CancellationToken cancellationToken)
        {
            try
            {
                var user = GetCurrentUserId();
                
                var query = _mapper.Map<PhotoQuery>(filterDto);

                var photos = await _binPhotoService.GetUserPhotosAsync(
                    user,
                    query,
                    cancellationToken);

                return Ok(photos);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
        
        [HttpPost]
        public async Task<ActionResult<BinPhotoResponse>> AddBinPhotoAsync([FromBody] AddPhotoRequest request)
        {
            var binPhoto = await _binPhotoService.AddBinPhotoAsync(request);

            return CreatedAtAction(nameof(AddBinPhotoAsync), new { id = binPhoto.Id }, binPhoto);
        }
        
        [HttpPost("UploadWithMetadata")]
        public async Task<ActionResult<BinPhotoResponse>> UploadWithMetadata(
            [FromForm] BinPhotoUploadRequest request,
            CancellationToken ct = default)
        {
            _logger.LogInformation("UploadWithMetadata вызван");

            var currentUserId = GetCurrentUserId();
            
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
                request.BinTypeCode, request.FillLevel, request.IsOutsideBin, request.Comment);

            _logger.LogInformation("Фото: FileName={FileName}, ContentType={ContentType}, Length={Length}",
                request.Photo.FileName, request.Photo.ContentType, request.Photo.Length);

            try
            {
                var binPhoto = await _binPhotoService.UploadPhotoAsync(request, currentUserId, ct);
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
        
        [AllowAnonymous]
        [HttpPost("Markers")]
        public async Task<ActionResult<IReadOnlyList<PhotoMarkerDTO>>> GetMarkersAsync()
        {
            try
            {
                var markers = await _binPhotoService.GetMarkersAsync();
                foreach (var marker in markers)
                {
                    _logger.LogInformation("Фото успешно загружено, Id={Id}", marker.Id);
                }
                return Ok(markers);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
