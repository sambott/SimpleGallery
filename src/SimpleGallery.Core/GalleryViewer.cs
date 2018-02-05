using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SimpleGallery.Core.Model;

namespace SimpleGallery.Core
{
    public class GalleryViewer<TMediaItem, TThumbItem, TIndexItem> : IGalleryViewer
        where TMediaItem : IGalleryItem
        where TThumbItem : IGalleryItem
        where TIndexItem : IIndexItem<TMediaItem>
    {
        private readonly IMediaStore<TMediaItem, TThumbItem, TIndexItem> _store;
        private readonly TimeSpan _cacheIndexFor;
        private readonly ILogger _logger;
        private readonly SemaphoreSlim _indexSemaphore = new SemaphoreSlim(0);
        private DateTime _indexCachedUntil = DateTime.MinValue;
        private List<TIndexItem> _index;

        public GalleryViewer(IMediaStore<TMediaItem, TThumbItem, TIndexItem> store, TimeSpan cacheIndexFor, ILogger logger)
        {
            _store = store;
            _cacheIndexFor = cacheIndexFor;
            _logger = logger;
        }

        private async Task<List<TIndexItem>> GetIndex()
        {
            await _indexSemaphore.WaitAsync();
            try
            {
                if (DateTime.Now > _indexCachedUntil)
                {
                    _logger.LogDebug("IndexCache expired, reloading...");
                    var indexEnumerable = await _store.GetAllIndexItems();
                    _index = indexEnumerable.ToList();
                    _indexCachedUntil = DateTime.Now + _cacheIndexFor;
                }
            }
            finally
            {
                _indexSemaphore.Release();
            }
            return _index;
        }

        public async Task<IEnumerable<IMediaItem>> ItemsInAlbum(string album)
        {
            var fqa = album ?? "/";
            if (!fqa.EndsWith("/"))
            {
                fqa += "/";
            }
            var index = await GetIndex();
            var groupedAtLevel = index
                .Where(i => i.Path.StartsWith(fqa))
                .ToDictionary(i => i.Path.Remove(0, fqa.Length))
                .GroupBy(relpathToItem => relpathToItem.Key.Split(new[] {'/'}, 2).First());
            var items = groupedAtLevel.Select<IGrouping<string, KeyValuePair<string, TIndexItem>>, IMediaItem>(group =>
            {
                var firstItem = group.First();
                if (group.Count() > 1 || firstItem.Key.Contains('/'))
                {
                    return new MediaAlbum(group.Key, firstItem.Value.Path, new HashSet<string>(group.Select(kvp => kvp.Value.Path)));
                }
                else
                {
                    return firstItem.Value;
                }
            });
            return items;
        }
    }
}