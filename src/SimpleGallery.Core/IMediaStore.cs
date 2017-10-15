using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleGallery.Core.Model;

namespace SimpleGallery.Core
{
    public interface IMediaStore<TMediaItem, TThumbItem, TIndexItem>
        where TMediaItem : IGalleryItem
        where TThumbItem : IGalleryItem
        where TIndexItem : IIndexItem<TMediaItem>
    {
        Task<IEnumerable<TMediaItem>> GetAllItems();

        Task<IEnumerable<TThumbItem>> GetAllThumbnails();

        Task<IEnumerable<TIndexItem>> GetAllIndexItems();

        Task<Stream> ReadItem(string path);

        Task UpdateThumbnail(TMediaItem thumbnail, Stream content);

        Task RemoveThumbnail(string path);

        Task UpdateIndex(TMediaItem item);

        Task RemoveIndex(string path);
    }
}