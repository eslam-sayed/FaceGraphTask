using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FaceGraph.API.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FaceGraph.API.Controllers
{
    [Route("api/container"), ApiController,
     Consumes("application/json", "multipart/form-data")]
    public class ContainerController : Controller
    {
        private readonly ILogger<ImagesController> _logger;
        private readonly IImagesService _imageService;

        public ContainerController(ILogger<ImagesController> logger, IImagesService imageService)
        {
            _logger = logger;
            _imageService = imageService;
        }

        [HttpDelete, Route("clear")]
        public async Task<IActionResult> ClearContainer()
        {
            await _imageService.DeleteImages();
            return Ok();
        }
    }
}