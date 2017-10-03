using System;
using System.Linq;
using System.Collections;
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
        private readonly string _bucketName;
        private readonly IAmazonS3 _client;
        private (int, int) _thumnailSize;
        
        public string BucketPrefix { get; }

        public AwsMediaStore(IAmazonS3 client, string bucketName, string bucketPrefix = "")
        {
            _client = client;
            _bucketName = bucketName;
            BucketPrefix = bucketPrefix;
        }

        public async Task ReadItem(string path, Stream output)
        {
            var response = await _client.GetObjectAsync(_bucketName, BucketPrefix + path);
            response.ResponseStream.CopyTo(output);
        }

        public IObservable<S3Object> GetS3Objects(string path = "")
        {
            return Observable.Create<S3Object>(async (obs, token) =>
            {
                var request = new ListObjectsV2Request
                {
                    BucketName = _bucketName,
                    Prefix = BucketPrefix + path,
                    MaxKeys = 150
                };
                ListObjectsV2Response response;
                do
                {
                    if (token.IsCancellationRequested) { break; }
                    response = await _client.ListObjectsV2Async(request);
                    response.S3Objects.ForEach(obs.OnNext);
                    request.ContinuationToken = response.NextContinuationToken;
                } while (response.IsTruncated == true);
            });
        }

        public Task<IGalleryAlbum> GetRootAlbum()
        {
            throw new NotImplementedException();
        }


        public async Task<IEnumerable<IMediaItem>> GetAllItems()
        {
            var images = await GetS3Objects()
                .Select(obj => new AwsGalleryImage(_thumnailSize, obj, this))
                .ToList().ToTask();
            return images;
        }

        public Task<IEnumerable<IMediaItem>> GetAllThumbnails()
        {
            throw new NotImplementedException();
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