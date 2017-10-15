using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SimpleGallery.Core.Model;
using SimpleGallery.Core.Model.MediaHandler;

namespace SimpleGallery.Core
{
    public sealed class GalleryBuilder<TMediaItem, TThumbItem, TIndexItem>
        where TMediaItem : IMediaItem
        where TThumbItem : IMediaItem
        where TIndexItem : IIndexItem<TMediaItem>
    {
        private readonly IMediaStore<TMediaItem, TThumbItem, TIndexItem> _store;
        private readonly IMediaHandler _mediaHandler;
        private Dictionary<string, TIndexItem> _indexPathDict;
        private Dictionary<string, TThumbItem> _thumbnailPathDict;
        private Dictionary<string, TMediaItem> _galleryContentPathDict;

        public GalleryBuilder(IMediaStore<TMediaItem, TThumbItem, TIndexItem> store, IMediaHandler mediaHandler)
        {
            _store = store;
            _mediaHandler = mediaHandler;
        }

        public async Task LoadItemSources()
        {
            var allMediaItems = await _store.GetAllItems().ConfigureAwait(false);
            var indexItems = await _store.GetAllIndexItems().ConfigureAwait(false);
            var thumbnailItems = await _store.GetAllThumbnails().ConfigureAwait(false);

            var galleryContentItems = await FilterByMediaHandler(allMediaItems).ConfigureAwait(false);

            _galleryContentPathDict = galleryContentItems.ToDictionary(item => item.Path);
            _indexPathDict = indexItems.ToDictionary(item => item.Path);
            _thumbnailPathDict = thumbnailItems.ToDictionary(item => item.Path);
        }

        private async Task<IEnumerable<TMediaItem>> FilterByMediaHandler(IEnumerable<TMediaItem> allMediaItems)
        {
            var results = new ConcurrentQueue<TMediaItem>();
            var tasks = allMediaItems.Select(
                async item =>
                {
                    if (await _mediaHandler.CanHandle(item))
                        results.Enqueue(item);
                });
            await Task.WhenAll(tasks);
            return results;
        }

        public async Task MakeThumbnailAndIndexConsistent()
        {
            var thumbnailDeltaPaths = _thumbnailPathDict.Keys.Except(_indexPathDict.Keys);
            var indexDeltaPaths = _indexPathDict.Keys.Except(_thumbnailPathDict.Keys).ToList();

            var thumbnailTasks = thumbnailDeltaPaths
                .Select(path => _thumbnailPathDict[path])
                .ToList()
                .Select(item => _store.RemoveThumbnail(item.Path));
            var indexTasks = indexDeltaPaths
                .Select(path => _indexPathDict[path])
                .ToList()
                .Select(item => _store.RemoveIndex(item.Path));
            await Task.WhenAll(thumbnailTasks.Concat(indexTasks)).ConfigureAwait(false);
        }

        public (IEnumerable<TMediaItem>, IEnumerable<TIndexItem>, IEnumerable<TMediaItem>)
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

        private IEnumerable<TMediaItem> GetUpdated(IEnumerable<TMediaItem> remaining)
        {
            return remaining.Where(item =>
            {
                var indexItem = _indexPathDict[item.Path];
                return indexItem.RequiresUpdate<TMediaItem>(item);
            });
        }

        public async Task Build()
        {
            await LoadItemSources().ConfigureAwait(false);

            await MakeThumbnailAndIndexConsistent();
            
            var (added, removed, remaining) = GetAddedRemovedRemaining();

            var updated = GetUpdated(remaining);

            foreach (var item in removed)
            {
                await RemoveIndexAndThumbnail(item).ConfigureAwait(false);
            }
            foreach (var item in updated)
            {
                await UpdateIndexAndThumbnail(item).ConfigureAwait(false);
            }
            foreach (var item in added)
            {
                await UpdateIndexAndThumbnail(item).ConfigureAwait(false);
            }
        }

        private async Task UpdateIndexAndThumbnail(TMediaItem item)
        {
            TThumbItem thumbnail;
            if (!await _mediaHandler.CanHandle(item))
            {
                throw new Exception("Should not get here");
            }
            
            using (var thumbnailStream = new MemoryStream())
            {
                    await _mediaHandler.GenerateThumbnail(item, thumbnailStream);
                    thumbnailStream.Seek(0, 0);
            thumbnail = await _store.UpdateThumbnail(item, thumbnailStream).ConfigureAwait(false);
            }
            
            await _store.UpdateIndex(item, thumbnail).ConfigureAwait(false);
        }

        private async Task RemoveIndexAndThumbnail(TIndexItem item)
        {
            await _store.RemoveIndex(item.Path).ConfigureAwait(false);
            await _store.RemoveThumbnail(item.Path).ConfigureAwait(false);
        }
    }
}