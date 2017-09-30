using System;
using System.Collections.Generic;
using System.IO;

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

        Stream GetMedia();
        Stream GetThumbnail();
        void GenerateThumbnail(Stream outputStream);
    }
}