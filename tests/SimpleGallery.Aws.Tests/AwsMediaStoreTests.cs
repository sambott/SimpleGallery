using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
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
            var mItemSource = new Mock<IS3ItemStore>();
            var mThumbSource = new Mock<IS3ItemStore>();
            var mIndexSource = new Mock<IDynamoDbIndex>();
            var store = new AwsMediaStore(mItemSource.Object, mThumbSource.Object, mIndexSource.Object, Mock.Of<ILogger>());
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

        [Fact]
        public async Task GetAllThumbsUsesThumbsS3Bucket()
        {
            var mItemSource = new Mock<IS3ItemStore>();
            var mThumbSource = new Mock<IS3ItemStore>();
            var mIndexSource = new Mock<IDynamoDbIndex>();
            var store = new AwsMediaStore(mItemSource.Object, mThumbSource.Object, mIndexSource.Object, Mock.Of<ILogger>());
            var s3Object = new S3Object
            {
                BucketName = "bucket",
                ETag = "hash23",
                Key = "path/to/item.jpg",
            };
            var s3ObjectList = new List<S3Object> {s3Object};
            mThumbSource.Setup(s => s.GetS3Objects(""))
                .Returns(s3ObjectList.ToObservable)
                .Verifiable();

            var result = await store.GetAllThumbnails();

            var expected = s3ObjectList.Select(o => new AwsGalleryImage(o, mItemSource.Object));
            mThumbSource.Verify();
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task GetAllIndexesUsesDynamo()
        {
            var mItemSource = new Mock<IS3ItemStore>();
            var mThumbSource = new Mock<IS3ItemStore>();
            var mIndexSource = new Mock<IDynamoDbIndex>();
            var store = new AwsMediaStore(mItemSource.Object, mThumbSource.Object, mIndexSource.Object, Mock.Of<ILogger>());
            var index = new IndexedAwsItem("name", "path", new HashSet<string>(), "hash", false);
            var indexList = new List<IndexedAwsItem> {index};
            mIndexSource.Setup(s => s.ScanItems())
                .Returns(indexList.ToObservable)
                .Verifiable();

            var result = await store.GetAllIndexItems();

            mIndexSource.Verify();
            Assert.Equal(indexList, result);
        }

        [Fact]
        public async Task ReadsItemFromS3()
        {
            var mItemSource = new Mock<IS3ItemStore>();
            var mThumbSource = new Mock<IS3ItemStore>();
            var mIndexSource = new Mock<IDynamoDbIndex>();
            var store = new AwsMediaStore(mItemSource.Object, mThumbSource.Object, mIndexSource.Object, Mock.Of<ILogger>());
            using (var stream = new MemoryStream())
            {
                mItemSource.Setup(s => s.ReadItem("123123"))
                    .ReturnsAsync(stream).Verifiable();

                using (var result = await store.ReadItem("123123"))
                {
                    mItemSource.Verify();
                    Assert.Equal(stream, result);                
                }
            }
        }

        [Fact]
        public async Task UpdateThumnailUsesThumbStore()
        {
            var mItemSource = new Mock<IS3ItemStore>();
            var mThumbSource = new Mock<IS3ItemStore>();
            var mIndexSource = new Mock<IDynamoDbIndex>();
            var mMediaItem = new Mock<IAwsMediaItem>();
            var store = new AwsMediaStore(mItemSource.Object, mThumbSource.Object, mIndexSource.Object, Mock.Of<ILogger>());
            using (var stream = new MemoryStream())
            {
                mMediaItem.Setup(i => i.Path).Returns("123");
                // ReSharper disable once AccessToDisposedClosure
                mThumbSource.Setup(s => s.WriteItem("123", stream))
                    .Returns(Task.CompletedTask).Verifiable();

                await store.UpdateThumbnail(mMediaItem.Object, stream);
                
                mThumbSource.Verify();
            }
        }

        [Fact]
        public async Task RemovesThumbFromS3()
        {
            var mItemSource = new Mock<IS3ItemStore>();
            var mThumbSource = new Mock<IS3ItemStore>();
            var mIndexSource = new Mock<IDynamoDbIndex>();
            var store = new AwsMediaStore(mItemSource.Object, mThumbSource.Object, mIndexSource.Object, Mock.Of<ILogger>());
            mThumbSource.Setup(s => s.DeleteItem("123123"))
                .Returns(Task.CompletedTask).Verifiable();

            await store.RemoveThumbnail("123123");
            
            mThumbSource.Verify();
        }

        [Fact]
        public async Task UpdateIndexUsesDynamo()
        {
            var mItemSource = new Mock<IS3ItemStore>();
            var mThumbSource = new Mock<IS3ItemStore>();
            var mIndexSource = new Mock<IDynamoDbIndex>();
            var mMediaItem = new Mock<IAwsMediaItem>();
            var store = new AwsMediaStore(mItemSource.Object, mThumbSource.Object, mIndexSource.Object, Mock.Of<ILogger>());
            mMediaItem.Setup(i => i.Name).Returns("123");
            mMediaItem.Setup(i => i.Path).Returns("456");
            mMediaItem.Setup(i => i.ChildPaths).Returns(new HashSet<string>());
            mMediaItem.Setup(i => i.Hash).Returns("789");
            mMediaItem.Setup(i => i.IsAlbum).Returns(false);
            
            mIndexSource.Setup(s => s.WriteItem(new IndexedAwsItem("123","456", new HashSet<string>(), "789", false)))
                .Returns(Task.CompletedTask).Verifiable();

            await store.UpdateIndex(mMediaItem.Object);

            mItemSource.Verify();
        }

        [Fact]
        public async Task RemovesIndexFromDynamo()
        {
            var mItemSource = new Mock<IS3ItemStore>();
            var mThumbSource = new Mock<IS3ItemStore>();
            var mIndexSource = new Mock<IDynamoDbIndex>();
            var store = new AwsMediaStore(mItemSource.Object, mThumbSource.Object, mIndexSource.Object, Mock.Of<ILogger>());
            mIndexSource.Setup(s => s.DeleteItem("123123"))
                .Returns(Task.CompletedTask).Verifiable();

            await store.RemoveIndex("123123");
            
            mIndexSource.Verify();
        }
    }
}