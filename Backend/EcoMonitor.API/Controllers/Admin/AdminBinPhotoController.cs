using EcoMonitor.API.Attributes;
using EcoMonitor.App.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoMonitor.API.Controllers.Admin;

[AdminApi]
[Authorize(Policy = "AdminPolicy")]
[Route("[controller]")]
public class AdminBinPhotoController(
    ILogger<AdminBinPhotoController> _logger,
    IBinPhotoService _binPhotoService)
    : BaseApiController<AdminBinPhotoController>(_logger)
{
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