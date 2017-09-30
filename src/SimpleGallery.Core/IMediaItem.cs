using System;
using System.Collections.Generic;
using System.IO;

namespace SimpleGallery.Core
{
    public interface IMediaItem
    {
        string Name { get; }
        string Path { get; }
        string ImageUrl { get; }
        string Thumbnail { get; }
        bool IsAlbum { get; }
        IEnumerable<IMediaItem> Children { get; }
        Stream GenerateThumbnail();
    }
}