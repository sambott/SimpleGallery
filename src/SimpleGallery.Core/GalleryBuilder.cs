using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SimpleGallery.Core.Model;
using SimpleGallery.Core.Model.MediaPreprocessor;

namespace SimpleGallery.Core
{
    public sealed class GalleryBuilder<TMediaItem, TThumbItem, TIndexItem> : IGalleryBuilder where TMediaItem : IGalleryItem
        where TThumbItem : IGalleryItem
        where TIndexItem : IIndexItem<TMediaItem>
    {
        private readonly IMediaStore<TMediaItem, TThumbItem, TIndexItem> _store;
        private readonly IMediaPreprocessor _mediaPreprocessor;
        private readonly ILogger _logger;
        private Dictionary<string, TIndexItem> _indexPathDict;
        private Dictionary<string, TThumbItem> _thumbnailPathDict;
        private Dictionary<string, TMediaItem> _galleryContentPathDict;

        public GalleryBuilder(IMediaStore<TMediaItem, TThumbItem, TIndexItem> store, IMediaPreprocessor mediaPreprocessor, ILogger logger)
        {
            _store = store;
            _mediaPreprocessor = mediaPreprocessor;
            _logger = logger;
        }

        public async Task LoadItemSources()
        {
            _logger.LogDebug("Loading indexes from media stores");
            var allMediaItems = await _store.GetAllItems().ConfigureAwait(false);
            var indexItems = await _store.GetAllIndexItems().ConfigureAwait(false);
            var thumbnailItems = await _store.GetAllThumbnails().ConfigureAwait(false);
            
            _logger.LogDebug("Filtering unknown media types");
            var galleryContentItems = await FilterByMediaPreprocessor(allMediaItems).ConfigureAwait(false);

            _galleryContentPathDict = galleryContentItems.ToDictionary(item => item.Path);
            _indexPathDict = indexItems.ToDictionary(item => item.Path);
            _thumbnailPathDict = thumbnailItems.ToDictionary(item => item.Path);
        }

        private async Task<IEnumerable<TMediaItem>> FilterByMediaPreprocessor(IEnumerable<TMediaItem> allMediaItems)
        {
            var results = new ConcurrentQueue<TMediaItem>();
            var tasks = allMediaItems.Select(
                async item =>
                {
                    if (await _mediaPreprocessor.CanHandle(item).ConfigureAwait(false))
                    {
                        results.Enqueue(item);
                    }
                });
            await Task.WhenAll(tasks).ConfigureAwait(false);
            return results;
        }

        public async Task MakeThumbnailAndIndexConsistent()
        {
            _logger.LogDebug("Dropping inconsistent Index and Thumbnail items");
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
            _logger.LogDebug("Calculating lists of Added, Removed and Preexisting items.");
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
            _logger.LogDebug("Looking for items modified since last gallery build.");
            return remaining.Where(item =>
            {
                var indexItem = _indexPathDict[item.Path];
                return indexItem.RequiresUpdate(item);
            });
        }

        public async Task Build()
        {
            _logger.LogInformation("Building Gallery");
            await LoadItemSources().ConfigureAwait(false);

            await MakeThumbnailAndIndexConsistent().ConfigureAwait(false);
            
            var (added, removed, remaining) = GetAddedRemovedRemaining();

            var updated = GetUpdated(remaining);

            foreach (var item in removed)
            {
                _logger.LogInformation($"Removing {item.Path}");
                await RemoveIndexAndThumbnail(item).ConfigureAwait(false);
            }
            foreach (var item in updated)
            {
                _logger.LogInformation($"Updating {item.Path}");
                await UpdateIndexAndThumbnail(item).ConfigureAwait(false);
            }
            foreach (var item in added)
            {
                _logger.LogInformation($"Adding {item.Path}");
                await UpdateIndexAndThumbnail(item).ConfigureAwait(false);
            }
        }

        private async Task UpdateIndexAndThumbnail(TMediaItem item)
        {
            if (!await _mediaPreprocessor.CanHandle(item).ConfigureAwait(false))
            {
                // Should not get here, unknown types were filtered out
                var msg = $"Could not create thumbnail for {item.Path}, no Preprocessor found.";
                _logger.LogCritical(msg);
                throw new InvalidOperationException(msg);
            }

            try
            {
                _logger.LogInformation($"Adding {item.Path} to index and thumbnails");
                _logger.LogDebug($"Building Thumbanil for {item.Path}");
                using (var itemStream = await _store.ReadItem(item.Path).ConfigureAwait(false))
                using (var thumbnailStream =
                    await _mediaPreprocessor.GenerateThumbnail(item, itemStream).ConfigureAwait(false))
                {
                    thumbnailStream.Seek(0, 0);
                    await _store.UpdateThumbnail(item, thumbnailStream).ConfigureAwait(false);
                }

                _logger.LogDebug($"Adding {item.Path} to Index");
                await _store.UpdateIndex(item).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown while creating index and thumbnail for {item.Path}:\n {ex}");
                // carry on....
            }
        }

        private async Task RemoveIndexAndThumbnail(TIndexItem item)
        {
            _logger.LogDebug($"Removing index and thumbnail for {item.Path}");
            await _store.RemoveIndex(item.Path).ConfigureAwait(false);
            await _store.RemoveThumbnail(item.Path).ConfigureAwait(false);
        }
    }
}