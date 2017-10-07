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

        public async Task ReadItem(string path, Stream output)
        {
            var response = await _client.GetObjectAsync(_bucketName, path);
            response.ResponseStream.CopyTo(output);
        }

        public IObservable<S3Object> GetS3Objects(string path = "")
        {
            return Observable.Create<S3Object>(async (obs, token) =>
            {
                var request = new ListObjectsV2Request
                {
                    BucketName = _bucketName,
                    Prefix = path,
                    MaxKeys = 150
                };
                ListObjectsV2Response response;
                do
                {
                    if (token.IsCancellationRequested) break;
                    response = await _client.ListObjectsV2Async(request);
                    response.S3Objects.ForEach(obs.OnNext);
                    request.ContinuationToken = response.NextContinuationToken;
                } while (response.IsTruncated);
            });
        }
    }
}