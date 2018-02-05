using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleGallery.Api.Models;
using SimpleGallery.Core;

namespace SimpleGallery.Api.Controllers
{
    [Route("api/[controller]")]
    public class AlbumController : Controller
    {
        private readonly IGalleryViewer _galleryViewer;
        private readonly AppOptions _config;
        private readonly ILogger<GalleryBuilderService> _logger;

        public AlbumController(IGalleryViewer galleryViewer, IOptions<AppOptions> options, ILogger<GalleryBuilderService> logger)
        {
            _galleryViewer = galleryViewer;
            _config = options.Value;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<Album>> Get()
        {
            var items = await _galleryViewer.ItemsInAlbum("");
            return items.Select(i => new Album() {Title = i.Path});
        }
    }
}