using System;
using System.IO;
using System.Threading.Tasks;

namespace SimpleGallery.Core.Model.MediaHandler
{
    public interface IMediaHandler
    {
        Task<bool> CanHandle(IGalleryItem item);
        Task<Stream> GenerateThumbnail(IGalleryItem item, Stream input);
        int Priority { get; }
    }
}