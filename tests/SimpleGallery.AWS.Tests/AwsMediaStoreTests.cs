using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Amazon.S3.Model;
using Moq;
using SimpleGallery.Aws.Model;
using Xunit;

namespace SimpleGallery.Aws.Tests
{
    public class AwsMediaStoreTests
    {
        [Fact]
        public async Task GetAllItemsUsesItemS3Bucket()
        {
            var mItemSource = new Mock<IS3Handler>();
            var mThumbSource = new Mock<IS3Handler>();
            var mIndexSource = new Mock<IDynamoDbHandler>();
            var store = new AwsMediaStore(mItemSource.Object, mThumbSource.Object, mIndexSource.Object);
            var s3Object = new S3Object
            {
                BucketName = "bucket",
                ETag = "hash23",
                Key = "path/to/item.jpg",
            };
            var s3ObjectList = new List<S3Object> {s3Object};
            mItemSource.Setup(s => s.GetS3Objects(""))
                .Returns(s3ObjectList.ToObservable)
                .Verifiable();
            
            var result = await store.GetAllItems();

            var expected = s3ObjectList.Select(o => new AwsGalleryImage(o, mItemSource.Object));
            mItemSource.Verify();
            Assert.Equal(expected, result);
        }
    }
}