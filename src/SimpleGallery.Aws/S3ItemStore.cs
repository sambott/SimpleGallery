using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;

namespace SimpleGallery.Aws
{
    public sealed class S3ItemStore : IS3ItemStore
    {
        private readonly string _bucketName;
        private readonly TimeSpan _linkTtl;
        private readonly ILogger _logger;
        private readonly IAmazonS3 _client;

        public S3ItemStore(IAmazonS3 client, string bucketName, TimeSpan linkTtl, ILogger logger)
        {
            _client = client;
            _bucketName = bucketName;
            _linkTtl = linkTtl;
            _logger = logger;
        }

        public async Task<Stream> ReadItem(string path)
        {
            _logger.LogDebug($"Reading {path} from S3");
            var response = await _client.GetObjectAsync(_bucketName, path);
            return response.ResponseStream;
        }

        public async Task WriteItem(string path, Stream content)
        {
            _logger.LogDebug($"Writing {path} to S3");
            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = path,
                InputStream = content,
            };
            await _client.PutObjectAsync(request);
        }

        public async Task DeleteItem(string path)
        {
            _logger.LogDebug($"Deleting {path} from S3");
            await _client.DeleteObjectAsync(_bucketName, path);
        }

        public IObservable<S3Object> GetS3Objects(string path)
        {
            _logger.LogDebug($"Creating listing observable for S3 from {path}.");
            return Observable.Create<S3Object>(async (obs, token) =>
            {
                ListObjectsV2Response response;
                var request = new ListObjectsV2Request
                {
                    BucketName = _bucketName,
                    Prefix = path,
                    MaxKeys = 100,
                };
                do
                {
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    _logger.LogDebug($"Requesting S3 keys from continuation \"{request.ContinuationToken ?? ""}\" at {path}.");
                    response = await _client.ListObjectsV2Async(request, token);
                    response.S3Objects.ForEach(obs.OnNext);
                    request.ContinuationToken = response.NextContinuationToken;
                } while (response.IsTruncated);
                obs.OnCompleted();
            });
        }

        public string UrlForPath(string path)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Expires = DateTime.Now + _linkTtl,
                Key = path,
                Protocol = Protocol.HTTPS,    
            };
            return _client.GetPreSignedURL(request);
        }
    }
}