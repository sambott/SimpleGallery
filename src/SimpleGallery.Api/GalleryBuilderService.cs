using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleGallery.Api.Models;
using SimpleGallery.Core;

namespace SimpleGallery.Api
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class GalleryBuilderService : HostedService
    {
        private readonly IGalleryBuilder _galleryBuilder;
        private readonly ILogger _logger;
        private readonly GalleryBuilderOptions _config;

        public GalleryBuilderService(IGalleryBuilder galleryBuilder, IOptions<AppOptions> options, ILogger<GalleryBuilderService> logger)
        {
            _galleryBuilder = galleryBuilder;
            _logger = logger;
            _config = options.Value.GalleryBuilder;
        }
        
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("Triggering Gallery Build");
                await _galleryBuilder.Build();
                await Task.Delay(_config.RebuildIntervalSeconds, cancellationToken);
            }
        }
    }
}