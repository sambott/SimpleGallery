using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using SimpleGallery.Aws.Media;
using SimpleGallery.Core;
using SimpleGallery.Core.Media;
using SimpleGallery.Core.Media.MediaHandler;

namespace SimpleGallery.Aws
{
    public sealed class AwsMediaStore : IMediaStore
    {
        private readonly IMediaHandler _mediaHandler;
        private readonly IS3Handler _itemSource;
        private readonly IS3Handler _thumbnailSource;
        private readonly IDynamoDbHandler _indexSource;

        public AwsMediaStore(IMediaHandler mediaHandler, IS3Handler itemSource, IS3Handler thumbnailSource, IDynamoDbHandler indexSource)
        {
            _mediaHandler = mediaHandler;
            _itemSource = itemSource;
            _thumbnailSource = thumbnailSource;
            _indexSource = indexSource;
        }

        public async Task<IEnumerable<IMediaItem>> GetAllItems()
        {
            var images = await _itemSource.GetS3Objects("")
                .Select(obj => new AwsGalleryImage(obj, this))
                .ToList().ToTask();
            return images;
        }

        public async Task<IEnumerable<IMediaItem>> GetAllThumbnails()
        {
            var images = await _thumbnailSource.GetS3Objects("")
                .Select(obj => new AwsGalleryImage(obj, this))
                .ToList().ToTask();
            return images;
        }

        public async Task<IEnumerable<IMediaItem>> GetAllIndexItems()
        {
            var indexItems = await _indexSource.ScanItems().ToList().ToTask();
            return indexItems;
        }

        public async Task UpdateThumbnail(IMediaItem item)
        {
            using (var thumbnailStream = new MemoryStream())
            {
                if (await _mediaHandler.CanHandle(item))
                {
                    await _mediaHandler.WriteThumbnail(item, thumbnailStream);
                }
                thumbnailStream.Seek(0, 0);
                await _thumbnailSource.WriteItem(item.Path, thumbnailStream);
            }
        }

        public async Task RemoveThumbnail(IMediaItem item)
        {
            await _thumbnailSource.DeleteItem(item.Path);
        }

        public async Task UpdateIndex(IMediaItem item)
        {
            await _indexSource.WriteItem(item);
        }

        public async Task RemoveIndex(IMediaItem item)
        {
            await _indexSource.DeleteItem(item);
        }
    }
}