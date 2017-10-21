using System.Collections.Generic;
using Amazon.S3.Model;
using Moq;
using SimpleGallery.Aws.Model;
using Xunit;

namespace SimpleGallery.Aws.Tests
{
    public class IndexedAwsItemTests
    {
        [Fact]
        public void IndexComparesToItem()
        {
            var s3Object = new S3Object
            {
                BucketName = "bucket",
                ETag = "hash23",
                Key = "path/to/item.jpg",
            };
            var s3Handler = new Mock<IS3Handler>();
            var item = new AwsGalleryImage(s3Object, s3Handler.Object);
            
            var index = new IndexedAwsItem("item.jpg", "path/to/item.jpg", new HashSet<string>(), "hash23", false);
            
            Assert.False(index.RequiresUpdate(item));
        }
    }
}