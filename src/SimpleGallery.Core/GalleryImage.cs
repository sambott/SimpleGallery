using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SixLabors.ImageSharp;

namespace SimpleGallery.Core
{
    public abstract class GalleryImage : IMediaItem
    {
        private readonly (int, int) _thumbnailSize;
        public abstract string Name { get; }
        public abstract string Path { get; }
        public abstract string MediaUrl { get; }
        public abstract string ThumbnailUrl { get; }
        
        public abstract Task<Stream> GetMedia();
        public abstract Task<Stream> GetThumbnail();
        
        public async Task GenerateThumbnail(Stream outputStream)
        {
            using (Stream imageStream = await GetMedia())
            using (Image<Rgba32> image = Image.Load(imageStream, out var format))
            {
                image.Mutate(ctx => ctx.Resize(_thumbnailSize.Item1, _thumbnailSize.Item2));
                image.Save(outputStream, format);
            }
        }

        public bool IsAlbum => false;
        public IEnumerable<IMediaItem> Children => Enumerable.Empty<IMediaItem>();
        public IEnumerator<IMediaItem> GetEnumerator() => Children.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        protected GalleryImage((int, int) thumbnailSize)
        {
            _thumbnailSize = thumbnailSize;
        }
    }
}