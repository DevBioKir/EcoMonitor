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

        public BinPhotoController(
            IBinPhotoService binPhotoService,
            IMapper mapper)
        {
            _binPhotoService = binPhotoService;
            _mapper = mapper;
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
    }
}
