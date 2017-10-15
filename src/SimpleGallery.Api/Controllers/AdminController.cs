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
        private readonly IGalleryBuilder _builder;

        public AdminController(IGalleryBuilder builder)
        {
            _builder = builder;
        }

        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            await _builder.Build();
            return new[] {"123"};
        }
    }
}