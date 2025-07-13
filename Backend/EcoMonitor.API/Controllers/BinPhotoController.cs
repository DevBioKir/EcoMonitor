using EcoMonitor.App.Services;
using EcoMonitor.Contracts.Contracts;
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
            IWebHostEnvironment env)
        {
            _binPhotoService = binPhotoService;
            _mapper = mapper;
            _env = env;
        }

        [HttpGet]
        public async Task<ActionResult<ICollection<BinPhotoResponse>>> GetAllBinPhotosAsync()
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
        public async Task<ActionResult<BinPhotoResponse>> UploadWithMetadata(/*[FromBody] BinPhotoUploadRequest request*/
            [FromForm] IFormFile photo,
            [FromForm] string binType,
            [FromForm] int fillLevel,
            [FromForm] bool isOutsideBin,
            [FromForm] string? comment)
        {
            var request = new BinPhotoUploadRequest(
                Photo: photo,
                BinType: binType,
                FillLevel: fillLevel,
                IsOutsideBin: isOutsideBin,
                Comment: comment);
            try
            {
                var binPhoto = await _binPhotoService.UploadImage(request);
                return CreatedAtAction(nameof(AddBinPhotoAsync), new { id = binPhoto.Id }, binPhoto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке фото с метаданными");
                return StatusCode(500, "Ошибка при обработке изображения.");
            }
        }
    }
}
