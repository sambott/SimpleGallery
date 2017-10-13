using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SimpleGallery.Core;

namespace SimpleGallery.Api.Controllers
{
    [Route("api/[controller]")]
    public class AdminController
    {
        private readonly IMediaStore _store;

        public AdminController(IMediaStore store)
        {
            _store = store;
        }

        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            var builder = new GalleryBuilder(_store);
            await builder.Build();
            return new[] {"123"};
        }
    }
}