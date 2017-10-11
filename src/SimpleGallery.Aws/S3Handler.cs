using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;

namespace SimpleGallery.Aws
{
    public sealed class S3Handler : IS3Handler
    {
        private readonly string _bucketName;
        private readonly IAmazonS3 _client;

        public S3Handler(IAmazonS3 client, string bucketName)
        {
            _client = client;
            _bucketName = bucketName;
        }

        public async Task<Stream> ReadItem(string path)
        {
            var response = await _client.GetObjectAsync(_bucketName, path);
            return response.ResponseStream;
        }

        public async Task WriteItem(string path, Stream content)
        {
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
            await _client.DeleteObjectAsync(_bucketName, path);
        }

        public IObservable<S3Object> GetS3Objects(string path = "")
        {
            return Observable.Create<S3Object>(async (obs, token) =>
            {
                ListObjectsV2Response response;
                var request = new ListObjectsV2Request
                {
                    BucketName = _bucketName,
                    Prefix = path,
                    MaxKeys = 150
                };
                do
                {
                    if (token.IsCancellationRequested) break;
                    response = await _client.ListObjectsV2Async(request);
                    response.S3Objects.ForEach(obs.OnNext);
                    request.ContinuationToken = response.NextContinuationToken;
                } while (response.IsTruncated);
                obs.OnCompleted();
            });
        }
    }
}