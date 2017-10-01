using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;

namespace SimpleGallery.Aws
{
    public class AwsMediaStore
    {
        private readonly string _bucketName;
        private readonly IAmazonS3 _client;
        private readonly string _keyPrefix;

        public AwsMediaStore(IAmazonS3 client, string bucketName, string keyPrefix)
        {
            this._client = client;
            _bucketName = bucketName;
            _keyPrefix = keyPrefix;
        }

        IObservable<S3Object> GetItems()
        {
            return Observable.Create<S3Object>(async (obs, token) =>
            {
                var request = new ListObjectsV2Request
                {
                    BucketName = _bucketName,
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
    }
}