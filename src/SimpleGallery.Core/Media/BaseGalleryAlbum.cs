using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SimpleGallery.Core.Media
{
    public abstract class BaseGalleryAlbum : IMediaItem
    {
        public abstract string Name { get; }
        public abstract string Path { get; }
        public abstract string MediaUrl { get; }
        public abstract string ThumbnailUrl { get; }
        public abstract IEnumerable<IMediaItem> Children { get; }
        
        public abstract Task<Stream> GetMedia();
        public abstract Task<Stream> GetThumbnail();
        public bool IsAlbum => true;

        public IEnumerator<IMediaItem> GetEnumerator() => Children.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}