using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SixLabors.ImageSharp;

namespace SimpleGallery.Core.Model.MediaHandler
{
    public sealed class PhotoHandler : IMediaHandler
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

        public Task<bool> CanHandle(IMediaItem item)
        {
            var extensionPosition = item.Path.LastIndexOf('.');
            var extension = item.Path.Substring(extensionPosition + 1);
            return Task.FromResult(
                _supported.Contains(extension.ToLowerInvariant())
            );
        }

        public Task<Stream> GenerateThumbnail(IMediaItem item, Stream input)
        {
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