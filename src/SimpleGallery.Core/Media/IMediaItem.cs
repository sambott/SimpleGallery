using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SimpleGallery.Core.Media
{
    public interface IMediaItem
    {
        string Name { get; }
        string Path { get; }
        string MediaUrl { get; }
        string ThumbnailUrl { get; }
        bool IsAlbum { get; }
        ISet<string> ChildPaths { get; }

        Task<Stream> GetMedia();
        Task<Stream> GetThumbnail();
        
    }
}