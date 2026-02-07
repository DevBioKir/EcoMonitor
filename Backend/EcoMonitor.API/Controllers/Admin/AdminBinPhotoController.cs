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

namespace EcoMonitor.API.Controllers.Admin;

[AdminApi]
[Authorize(Policy = "AdminPolicy")]
[Route("[controller]")]
public class AdminBinPhotoController(
    ILogger<AdminBinPhotoController> _logger,
    IBinPhotoService _binPhotoService,
    IMapper _mapper)
    : BaseApiController<AdminBinPhotoController>(_logger)
{
    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) throw new UnauthorizedAccessException("User is not authenticated");
        return Guid.Parse(userIdClaim.Value);
    }
    
    [HttpPost("UploadWithMetadata")]
    public async Task<ActionResult<BinPhotoResponse>> UploadWithMetadata(
        [FromForm] BinPhotoUploadRequest request,
        CancellationToken ct = default)
    {
        _logger.LogInformation("UploadWithMetadata вызван");
        
        _logger.LogInformation(
            "Raw FillLevel from form = {FillLevel}",
            Request.Form["FillLevel"]
        );
        
        _logger.LogInformation(
            "Parsed FillLevel = {FillLevel}",
            request.FillLevel
        );

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
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePhotoAsync(
        string id, [FromForm] UpdatePhotoRequest request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(id, out var photoId))
        {
            return BadRequest(new { error = "Invalid User ID format" });
        }
        _logger.LogInformation("Контроллер получил JSON: {@Request}", request);
        
        var currentUserId = GetCurrentUserId();
        
        await _binPhotoService.UpdatePhotoAsync(currentUserId, photoId, request, cancellationToken);
        _logger.LogInformation("Контроллер ПОСЛЕ: {@Request}", request);
        
        return NoContent();
    }
    
    
    [HttpGet("GetAllPhotos")]
    public async Task<ActionResult<IReadOnlyList<BinPhotoResponse>>> GetAllBinPhotosAsync()
    {
        var binPhotos = await _binPhotoService.GetAllBinPhotosAsync();
        var responseBinPhotos = _mapper.Map<List<BinPhotoResponse>>(binPhotos);

        return Ok(responseBinPhotos);
    }
    
    [HttpDelete("{binPhotoId:guid}")]
    public async Task<ActionResult<Guid>> DeleteBinPhotoAsync(Guid binPhotoId)
    {
        try
        {
            await _binPhotoService.DeleteBinPhotoAsync(binPhotoId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при удалении фото из базы");
            return StatusCode(500, "Ошибка при удалении фотографии.");
        }
    }
}