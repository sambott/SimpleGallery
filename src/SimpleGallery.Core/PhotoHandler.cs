using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SixLabors.ImageSharp;

namespace SimpleGallery.Core
{
    public sealed class PhotoHandler : AbstractMediaHandler
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

        private ImageSize _thumbnailSize;

        public PhotoHandler(ImageSize thumbnailSize, int priority )
        {
            _thumbnailSize = thumbnailSize;
            Priority = priority;
        }

        public override Task<bool> CanHandle(IMediaItem item)
        {
            var extensionPosition = item.Path.LastIndexOf('.');
            var extension = item.Path.Substring(extensionPosition + 1);
            return Task.FromResult(
                _supported.Contains(extension.ToLowerInvariant())
            );
        }

        public override async Task WriteThumbnail(IMediaItem item, Stream output)
        {
            using (var imageStream = await item.GetMedia())
            using (var image = Image.Load(imageStream, out var format))
            {
                image.Mutate(ctx => ctx.Resize(_thumbnailSize.Width, _thumbnailSize.Height));
                image.Save(output, format);
            }
        }

        public override int Priority { get; }
    }
}