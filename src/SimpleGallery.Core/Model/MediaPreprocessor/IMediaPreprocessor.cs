using System.IO;
using System.Threading.Tasks;

namespace SimpleGallery.Core.Model.MediaPreprocessor
{
    public interface IMediaPreprocessor
    {
        Task<bool> CanHandle(IGalleryItem item);
        Task<Stream> GenerateThumbnail(IGalleryItem item, Stream input);
        int Priority { get; }
    }
}