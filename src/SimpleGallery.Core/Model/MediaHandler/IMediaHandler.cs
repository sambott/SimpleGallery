using System;
using System.IO;
using System.Threading.Tasks;

namespace SimpleGallery.Core.Model.MediaHandler
{
    public interface IMediaHandler
    {
        Task<bool> CanHandle(IMediaItem item);
        Task<Stream> GenerateThumbnail(IMediaItem item, Stream input);
        int Priority { get; }
    }
}