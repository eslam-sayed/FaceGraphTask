using System.Collections.Generic;
using System.Threading.Tasks;
using FaceGraph.API.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.IO;
using FaceGraph.API.Dtos;

namespace FaceGraph.API.Controllers
{
    [Route("api/images"), ApiController,
    Consumes("application/json","multipart/form-data")]
    public class ImagesController : Controller
    {
        private readonly ILogger<ImagesController> _logger;
        private readonly IImagesService _imageService;

        public ImagesController(ILogger<ImagesController> logger, IImagesService imageService)
        {
            _logger = logger;
            _imageService = imageService;
        }

        [HttpGet]
        public async Task<IEnumerable<ImagesResponse>> Get()
        {
            var images = await _imageService.GetImages();
            var response = images.Select(img => new ImagesResponse
            {
                Uri = img.Uri.AbsoluteUri,
                FileName = Path.GetFileName(img.Uri.AbsolutePath)
            });
            return response;
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadImage(List<IFormFile> files)
        {
            _logger.LogTrace("Start getting the uploaded files from the request");

            if (files.Count <= 0)
            {
                _logger.LogWarning("The request does not have any file to upload");
                return BadRequest();
            }

            if (files.Any(f=> !f.ContentType.StartsWith("image")))
            {
                _logger.LogWarning("The request has files not images");
                return BadRequest();
            }

            foreach (var file in files)
            {
                _logger.LogInformation($"uploading file {file.FileName}...");
                await _imageService.UploadImage(file.OpenReadStream(), file.FileName);
            }
            _logger.LogTrace("Uploading the images done succesfully");
            return Ok();
        }

        [HttpDelete("{fileName}")]
        public async Task<IActionResult> DeleteImage([FromRoute]string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return BadRequest("Image fileName not provided!");
            }
            await _imageService.DeleteImage(fileName);
            return Ok();
        }

    }
}