using System;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Moq;
using Xunit;

namespace SimpleGallery.Aws.Tests
{
    public class S3HandlerTests
    {
        private const string BucketName = "abc213";
        private const string Key = "321cba";
        private static readonly TimeSpan LinkTtl = TimeSpan.FromMinutes(1);

        [Fact]
        public async Task ReadItemCallsClient()
        {
            var client = new Mock<IAmazonS3>();
            using (var tempStream = new MemoryStream())
            {
                client.Setup(c =>
                        c.GetObjectAsync(BucketName, Key, It.IsAny<CancellationToken>())
                    )
                    .ReturnsAsync(new GetObjectResponse() {ResponseStream = tempStream})
                    .Verifiable();
                var handler = new S3Handler(client.Object, BucketName, LinkTtl);

                using (var responseStream = await handler.ReadItem(Key))
                {
                    client.Verify();
                    Assert.Equal(tempStream, responseStream);
                }
            }
        }

        [Fact]
        public async Task DeleteItemCallsClient()
        {
            var client = new Mock<IAmazonS3>();
            client.Setup(c =>
                    c.DeleteObjectAsync(BucketName, Key, It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(new DeleteObjectResponse())
                .Verifiable();
            var handler = new S3Handler(client.Object, BucketName, LinkTtl);

            await handler.DeleteItem(Key);

            client.Verify();
        }

        [Fact]
        public async Task WriteItemCallsCLient()
        {
            var client = new Mock<IAmazonS3>();

            using (var tempStream = new MemoryStream())
            {
                client.Setup(c =>
                        c.PutObjectAsync(
                            It.Is<PutObjectRequest>(r =>
                                r.BucketName == BucketName &&
                                r.Key == Key &&
                                r.InputStream == tempStream
                            ),
                            It.IsAny<CancellationToken>()
                        ))
                    .ReturnsAsync(new PutObjectResponse())
                    .Verifiable();
                var handler = new S3Handler(client.Object, BucketName, LinkTtl);
                await handler.WriteItem(Key, tempStream);
            }
            client.Verify();
        }

        [Fact]
        public void UrlRequestCallsClient()
        {
            var client = new Mock<IAmazonS3>();
            client.Setup(c =>
                    c.GetPreSignedURL(It.Is<GetPreSignedUrlRequest>(r =>
                        r.BucketName == BucketName &&
                        r.Expires > DateTime.Now &&
                        r.Expires < DateTime.Now + 2 * LinkTtl &&
                        r.Key == Key &&
                        r.Protocol == Protocol.HTTPS
                    ))
                )
                .Returns("test_string")
                .Verifiable();
            var handler = new S3Handler(client.Object, BucketName, LinkTtl);

            var url = handler.UrlForPath(Key);

            client.Verify();
            Assert.Equal("test_string", url);
        }
    }
}