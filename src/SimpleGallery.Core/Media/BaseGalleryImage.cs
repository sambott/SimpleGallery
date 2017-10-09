using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleGallery.Core.Media
{
    public abstract class BaseGalleryImage : IMediaItem
    {
        public abstract string Name { get; }
        public abstract string Path { get; }
        public abstract string MediaUrl { get; }
        public abstract string ThumbnailUrl { get; }

        public abstract Task<Stream> GetMedia();
        public abstract Task<Stream> GetThumbnail();

        public bool IsAlbum => false;
        public IEnumerable<IMediaItem> Children => Enumerable.Empty<IMediaItem>();

        public IEnumerator<IMediaItem> GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}