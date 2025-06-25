using Microsoft.AspNetCore.Mvc;

namespace EcoMonitor.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageUploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public ImageUploadController(
            IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpPost]
        public async Task<ActionResult> UploadImage([FromForm] IFormFile image)
        {
            if (image == null || image.Length < 0) return BadRequest("File not uploaded");

            if (!image.ContentType.StartsWith("image/")) return BadRequest("The uploaded file is not an image.");

            var imagesFolder = Path.Combine(_env.WebRootPath, "Photos");

            if(!Directory.Exists(imagesFolder))
                Directory.CreateDirectory(imagesFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(image.FileName)}";
            var filePath = Path.Combine(imagesFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            };

            return Ok(new 
            {
                fileName = uniqueFileName, 
                filePath = $"/Photos/{uniqueFileName}"
            });
        }
    }
}
