using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleGallery.Core.Media;

namespace SimpleGallery.Core
{
    public interface IMediaStore
    {
        Task<IEnumerable<IMediaItem>> GetAllItems();

        Task<IEnumerable<IMediaItem>> GetAllThumbnails();

        Task<IEnumerable<IMediaItem>> GetIndexItems();

        Task UpdateThumbnail(IMediaItem item);

        Task RemoveThumbnail(IMediaItem item);

        Task UpdateIndex(IMediaItem item);

        Task RemoveIndex(IMediaItem item);
    }
}