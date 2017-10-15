using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleGallery.Core.Model;

namespace SimpleGallery.Core
{
    public interface IMediaStore<TMediaItem, TThumbItem, TIndexItem>
        where TMediaItem : IMediaItem
        where TThumbItem : IMediaItem
        where TIndexItem : IIndexItem<TMediaItem>
    {
        Task<IEnumerable<TMediaItem>> GetAllItems();

        Task<IEnumerable<TThumbItem>> GetAllThumbnails();

        Task<IEnumerable<TIndexItem>> GetAllIndexItems();

        Task<TThumbItem> UpdateThumbnail(TMediaItem thumbnail, Stream content);

        Task RemoveThumbnail(string path);

        Task<TIndexItem> UpdateIndex(TMediaItem item, TThumbItem thumbnail);

        Task RemoveIndex(string path);
    }
}