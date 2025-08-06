using EcoMonitor.App.Services;
using EcoMonitor.Contracts.Contracts;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace EcoMonitor.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BinTypeController : ControllerBase
    {
        private readonly IBinTypeService _binTypeService;
        private readonly IMapper _mapper;
        private readonly ILogger<BinTypeController> _logger;

        public BinTypeController(
            IBinTypeService binTypeService,
            IMapper mapper,
            ILogger<BinTypeController> logger)
        {
            _binTypeService = binTypeService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet("GetAllBinTypes")]
        public async Task<ActionResult<IReadOnlyList<BinTypeResponse>>> GetAllBinTypesAsync()
        {
            var binTypes = await _binTypeService.GetAllBinTypesAsync();
            var responseBinType = _mapper.Map<IReadOnlyList<BinTypeResponse>>(binTypes);

            return Ok(responseBinType);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BinTypeResponse>> GetBinTypeByIdAsync(Guid id)
        {
            try
            {
                var binType = await _binTypeService.GetBinTypeByIdAsync(id);
                var responseBinType = _mapper.Map<BinTypeResponse>(binType);

                return Ok(responseBinType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при поиске типа баков в базе");
                return StatusCode(500, "Ошибка при поиске типа баков в базе.");
            }
        }

        [HttpPost("AddBinTypeAsync")]
        public async Task<ActionResult<BinTypeResponse>> AddBinTypeAsync([FromBody] BinTypeRequest request)
        {
            try
            {
                var binType = await _binTypeService.AddBinTypeAsync(request);
            return CreatedAtAction(
                actionName: nameof(GetBinTypeByIdAsync),
                controllerName: "BinType", // без "Controller"
                routeValues: new { id = binType.Id },
                value: binType);
        }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании BinType");
                return StatusCode(500, new { message = ex.Message});
            }
        }



    }
}
