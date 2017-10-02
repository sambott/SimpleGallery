using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SimpleGallery.Core
{
    public interface IMediaItem : IEnumerable<IMediaItem>
    {
        //TODO maybe store thumbnail size in DB so can update if config changes
        string Name { get; }
        string Path { get; }
        string MediaUrl { get; }
        string ThumbnailUrl { get; }
        bool IsAlbum { get; }
        IEnumerable<IMediaItem> Children { get; } // async?

        Task<Stream> GetMedia();
        Task<Stream> GetThumbnail();
        Task GenerateThumbnail(Stream outputStream);
    }
}