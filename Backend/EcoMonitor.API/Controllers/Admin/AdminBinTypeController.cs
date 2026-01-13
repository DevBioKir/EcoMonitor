using EcoMonitor.API.Attributes;
using EcoMonitor.App.Services;
using EcoMonitor.Contracts.Contracts.BinType;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoMonitor.API.Controllers.Admin;

[AdminApi]
[Authorize(Policy = "AdminPolicy")]
[Route("[controller]")]
public class AdminBinTypeController (
    IBinTypeService _binTypeService,
    IMapper _mapper,
    ILogger<AdminBinTypeController> _logger)
    : ControllerBase
{
    [HttpGet("GetAllBinTypes")]
    public async Task<ActionResult<IReadOnlyList<BinTypeResponse>>> GetAllBinTypesAsync()
    {
        var binTypes = await _binTypeService.GetAllBinTypesAsync();
        var responseBinType = _mapper.Map<IReadOnlyList<BinTypeResponse>>(binTypes);

        return Ok(responseBinType);
    }
}