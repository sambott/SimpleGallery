using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using SimpleGallery.Aws.Model;
using SimpleGallery.Core;
using SimpleGallery.Core.Model;

namespace SimpleGallery.Aws
{
    public sealed class AwsMediaStore : IMediaStore<IAwsMediaItem, IAwsMediaItem, IAwsIndexItem<IAwsMediaItem>>
    {
        private readonly IS3ItemStore _itemSource;
        private readonly IS3ItemStore _thumbnailSource;
        private readonly IDynamoDbIndex _indexSource;
        private readonly ILogger _logger;

        public AwsMediaStore(IS3ItemStore itemSource, IS3ItemStore thumbnailSource, IDynamoDbIndex indexSource, ILogger logger)
        {
            _itemSource = itemSource;
            _thumbnailSource = thumbnailSource;
            _indexSource = indexSource;
            _logger = logger;
        }

        public async Task<IEnumerable<IAwsMediaItem>> GetAllItems()
        {
            _logger.LogDebug("Getting all MediaItem keys");
            var images = await _itemSource.GetS3Objects("")
                .Select(obj => new AwsGalleryImage(obj, _itemSource))
                .ToList().ToTask();
            _logger.LogDebug("Retrieved list of all MediaItems");
            return images;
        }

        public async Task<IEnumerable<IAwsMediaItem>> GetAllThumbnails()
        {
            _logger.LogDebug("Getting list of all Thumbnail keys");
            var images = await _thumbnailSource.GetS3Objects("")
                .Select(obj => new AwsGalleryImage(obj, _thumbnailSource))
                .ToList().ToTask();
            _logger.LogDebug("Retrieved list of all thumbnails");
            return images;
        }

        public async Task<IEnumerable<IAwsIndexItem<IAwsMediaItem>>> GetAllIndexItems()
        {
            _logger.LogDebug("Getting index from DynamoDB");
            var indexItems = await _indexSource.ScanItems().ToList().ToTask();
            _logger.LogDebug("Retrieved list of all items in index");
            return indexItems;
        }

        public async Task<Stream> ReadItem(string path)
        {
            _logger.LogDebug($"Reading full content for {path}");
            return await _itemSource.ReadItem(path);
        }

        public async Task UpdateThumbnail(IAwsMediaItem thumbnail, Stream content)
        {
            _logger.LogDebug($"Writing content for {thumbnail.Path} thumbnail");
            await _thumbnailSource.WriteItem(thumbnail.Path, content);
        }

        public async Task RemoveThumbnail(string itemPath)
        {
            _logger.LogDebug($"Removing thumbnail for {itemPath}");
            await _thumbnailSource.DeleteItem(itemPath);
        }

        public async Task UpdateIndex(IAwsMediaItem item)
        {
            _logger.LogDebug($"Writing index entry for {item.Path}");
            var indexItem = new IndexedAwsItem(item.Name, item.Path, item.ChildPaths, item.Hash, item.IsAlbum);
            await _indexSource.WriteItem(indexItem);
        }

        public async Task RemoveIndex(string itemPath)
        {
            _logger.LogDebug($"Removing index entry for {itemPath}");
            await _indexSource.DeleteItem(itemPath);
        }
    }
}