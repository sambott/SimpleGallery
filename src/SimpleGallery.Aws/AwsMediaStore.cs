using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using SimpleGallery.Core;

namespace SimpleGallery.Aws
{
    public class AwsMediaStore : IMediaStore
    {
        private readonly IS3Handler _itemSource;
        private readonly IS3Handler _thumbnailSource;
        private readonly IDynamoDBHandler _indexSource;

        public AwsMediaStore(IS3Handler itemSource, IS3Handler thumbnailSource, IDynamoDBHandler indexSource)
        {
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

        public Task<IEnumerable<IMediaItem>> GetIndexItems()
        {
            throw new NotImplementedException();
        }

        public Task UpdateThumbnail(IMediaItem item)
        {
            throw new NotImplementedException();
        }

        public Task RemoveThumbnail(IMediaItem path)
        {
            throw new NotImplementedException();
        }

        public Task UpdateIndex(IMediaItem item)
        {
            throw new NotImplementedException();
        }

        public Task RemoveIndex(IMediaItem item)
        {
            throw new NotImplementedException();
        }
    }
}