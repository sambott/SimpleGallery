using System;
using System.IO;
using System.Threading.Tasks;

namespace SimpleGallery.Core
{
    public interface IMediaHandler : IComparable<IMediaHandler>
    {
        Task<bool> CanHandle(IMediaItem item);
        Task WriteThumbnail(IMediaItem item, Stream output);
        int Priority { get; }
    }
}