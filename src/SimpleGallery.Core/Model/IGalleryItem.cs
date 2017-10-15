using System.Collections.Generic;

namespace SimpleGallery.Core.Model
{
    public interface IGalleryItem : IMediaItem
    {
        string Url { get; }
    }
}