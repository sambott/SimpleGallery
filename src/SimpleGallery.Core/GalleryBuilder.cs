using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimpleGallery.Core.Media;

namespace SimpleGallery.Core
{
    public sealed class GalleryBuilder
    {
        private readonly IMediaStore _store;
        private Dictionary<string, IMediaItem> _indexPathDict;
        private Dictionary<string, IMediaItem> _thumbnailPathDict;
        private Dictionary<string, IMediaItem> _galleryContentPathDict;

        public GalleryBuilder(IMediaStore store)
        {
            _store = store;
        }

        public async Task LoadItemSources()
        {
            var galleryContentItems = await _store.GetAllItems().ConfigureAwait(false);
            var indexItems = await _store.GetIndexItems().ConfigureAwait(false);
            var thumbnailItems = await _store.GetAllThumbnails().ConfigureAwait(false);

            _galleryContentPathDict = galleryContentItems.ToDictionary(item => item.Path);
            _indexPathDict = indexItems.ToDictionary(item => item.Path);
            _thumbnailPathDict = thumbnailItems.ToDictionary(item => item.Path);
        }

        public void MakeThumbnailAndIndexConsistent()
        {
            var thumbnailDeltaPaths = _thumbnailPathDict.Keys.Except(_indexPathDict.Keys);
            var indexDeltaPaths = _indexPathDict.Keys.Except(_thumbnailPathDict.Keys).ToList();

            thumbnailDeltaPaths
                .Select(path => _thumbnailPathDict[path])
                .ToList()
                .ForEach(async item => await _store.RemoveThumbnail(item));
            indexDeltaPaths
                .Select(path => _indexPathDict[path])
                .ToList()
                .ForEach(async item => await _store.RemoveIndex(item));
        }

        public (IEnumerable<IMediaItem>, IEnumerable<IMediaItem>, IEnumerable<IMediaItem>)
            GetAddedRemovedRemaining()
        {
            var addedPaths = _galleryContentPathDict.Keys.Except(_indexPathDict.Keys).ToList();
            var removedPaths = _indexPathDict.Keys.Except(_galleryContentPathDict.Keys);
            var remainingPaths = _galleryContentPathDict.Keys.Except(addedPaths);

            var added = addedPaths.Select(path => _galleryContentPathDict[path]);
            var removed = removedPaths.Select(path => _indexPathDict[path]);
            var remaining = remainingPaths.Select(path => _galleryContentPathDict[path]);

            return (added, removed, remaining);
        }

        private IEnumerable<IMediaItem> GetUpdated(IEnumerable<IMediaItem> remaining)
        {
            return remaining.Where(item =>
            {
                var indexItem = _indexPathDict[item.Path];
                return !Equals(item, indexItem);
            });
        }

        public async Task Build()
        {
            await LoadItemSources();

            MakeThumbnailAndIndexConsistent();
            
            var (added, removed, remaining) = GetAddedRemovedRemaining();

            var updated = GetUpdated(remaining);

            foreach (var item in removed)
            {
                await _store.RemoveIndex(item);
                await _store.RemoveThumbnail(item);
            }
            foreach (var item in updated)
            {
                await _store.UpdateThumbnail(item);
                await _store.UpdateIndex(item);
            }
            foreach (var item in added)
            {
                await _store.UpdateThumbnail(item);
                await _store.UpdateIndex(item);
            }
        }
    }
}