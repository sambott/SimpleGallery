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
using SimpleGallery.Aws.Model;
using SimpleGallery.Core;
using SimpleGallery.Core.Model;
using SimpleGallery.Core.Model.MediaHandler;

namespace SimpleGallery.Aws
{
    public sealed class AwsMediaStore : IMediaStore<IAwsMediaItem, IAwsMediaItem, IAwsIndexItem<IAwsMediaItem>>
    {
        private readonly IS3Handler _itemSource;
        private readonly IS3Handler _thumbnailSource;
        private readonly IDynamoDbHandler _indexSource;

        public AwsMediaStore(IS3Handler itemSource, IS3Handler thumbnailSource, IDynamoDbHandler indexSource)
        {
            _itemSource = itemSource;
            _thumbnailSource = thumbnailSource;
            _indexSource = indexSource;
        }

        public async Task<IEnumerable<IAwsMediaItem>> GetAllItems()
        {
            var images = await _itemSource.GetS3Objects("")
                .Select(obj => new AwsGalleryImage(obj, _itemSource))
                .ToList().ToTask();
            return images;
        }

        public async Task<IEnumerable<IAwsMediaItem>> GetAllThumbnails()
        {
            var images = await _thumbnailSource.GetS3Objects("")
                .Select(obj => new AwsGalleryImage(obj, _thumbnailSource))
                .ToList().ToTask();
            return images;
        }

        public async Task<IEnumerable<IAwsIndexItem<IAwsMediaItem>>> GetAllIndexItems()
        {
            var indexItems = await _indexSource.ScanItems().ToList().ToTask();
            return indexItems;
        }

        public async Task<Stream> ReadItem(string path)
        {
            return await _itemSource.ReadItem(path);
        }

        public async Task UpdateThumbnail(IAwsMediaItem thumbnail, Stream content)
        {
            await _thumbnailSource.WriteItem(thumbnail.Path, content);
        }

        public async Task RemoveThumbnail(string itemPath)
        {
            await _thumbnailSource.DeleteItem(itemPath);
        }

        public async Task UpdateIndex(IAwsMediaItem item)
        {
            var indexItem = new IndexedAwsItem(item.Name, item.Path, item.ChildPaths, item.Hash, item.IsAlbum);
            await _indexSource.WriteItem(indexItem);
        }

        public async Task RemoveIndex(string itemPath)
        {
            await _indexSource.DeleteItem(itemPath);
        }
    }
}