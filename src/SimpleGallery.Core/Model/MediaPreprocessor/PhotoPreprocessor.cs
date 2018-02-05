using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;

namespace SimpleGallery.Core.Model.MediaPreprocessor
{
    public sealed class PhotoPreprocessor : IMediaPreprocessor
    {
        private readonly HashSet<string> _supported = new HashSet<string>
        {
            "png",
            "jpeg",
            "jpg",
            "gif",
            "bitmap",
            "bmp",
        };

        private readonly ImageSize _thumbnailSize;
        private readonly ILogger _logger;

        public PhotoPreprocessor(ImageSize thumbnailSize, int priority, ILogger logger)
        {
            _thumbnailSize = thumbnailSize;
            _logger = logger;
            Priority = priority;
        }

        public Task<bool> CanHandle(IGalleryItem item)
        {
            var extensionPosition = item.Path.LastIndexOf('.');
            var extension = item.Path.Substring(extensionPosition + 1);
            return Task.FromResult(
                _supported.Contains(extension.ToLowerInvariant())
            );
        }

        public Task<Stream> GenerateThumbnail(IGalleryItem item, Stream input)
        {
            _logger.LogDebug($"Creating thumbnail for image {item.Path}");
            var output = new MemoryStream();
            using (var image = Image.Load(input, out var format))
            {
                image.Mutate(ctx => ctx.Resize(_thumbnailSize.Width, _thumbnailSize.Height));
                image.Save(output, format);
            }
            output.Seek(0, 0);
            return Task.FromResult<Stream>(output);
        }

        public int Priority { get; }
    }
}