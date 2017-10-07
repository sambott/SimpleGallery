using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SimpleGallery.Core
{
    public interface IMediaItem : IEnumerable<IMediaItem>
    {
        string Name { get; }
        string Path { get; }
        string MediaUrl { get; }
        string ThumbnailUrl { get; }
        bool IsAlbum { get; }
        IEnumerable<IMediaItem> Children { get; }

        Task<Stream> GetMedia();
        Task<Stream> GetThumbnail();
    }
}