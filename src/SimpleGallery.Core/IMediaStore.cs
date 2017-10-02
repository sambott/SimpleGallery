using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleGallery.Core
{
    public interface IMediaStore
    {
        Task<IGalleryAlbum> GetRootAlbum(); // needed?

        Task<IEnumerable<IMediaItem>> GetAllItems();

        Task<IEnumerable<IMediaItem>> GetAllThumbnails();

        Task UpdateThumbnail(IMediaItem item);

        Task RemoveThumbnail(IMediaItem item);

        Task UpdateIndex(IMediaItem item);

        Task RemoveIndex(IMediaItem item);
    }
}