using System.Collections.Generic;

namespace SimpleGallery.Core.Model
{
    public interface IMediaItem
    {
        string Name { get; }
        string Path { get; }
        bool IsAlbum { get; }
        ISet<string> ChildPaths { get; }
    }
}