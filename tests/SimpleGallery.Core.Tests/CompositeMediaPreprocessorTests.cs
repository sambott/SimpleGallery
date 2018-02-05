using System.IO;
using System.Threading.Tasks;
using Moq;
using SimpleGallery.Core.Model;
using SimpleGallery.Core.Model.MediaPreprocessor;
using Xunit;

namespace SimpleGallery.Core.Tests
{
    public class CompositeMediaPreprocessorTests
    {
        [Fact]
        public async Task CompositeHandlerDoesntHandleIfItsComponentsCant()
        {
            var mItem = new Mock<IGalleryItem>();
            var mHandler1 = new Mock<IMediaPreprocessor>();
            var mHandler2 = new Mock<IMediaPreprocessor>();
            mHandler1.Setup(h => h.Priority).Returns(1);
            mHandler2.Setup(h => h.Priority).Returns(2);
            mHandler1.Setup(h => h.CanHandle(It.IsAny<IGalleryItem>())).ReturnsAsync(false);
            mHandler2.Setup(h => h.CanHandle(It.IsAny<IGalleryItem>())).ReturnsAsync(false);
            var composite = new CompositeMediaPreprocessor
            {
                mHandler1.Object,
                mHandler2.Object,
            };
            
            Assert.False(await composite.CanHandle(mItem.Object));
        }
        
        [Fact]
        public async Task CompositeHandlerHandlesIfItsComponentsCan()
        {
            var mItem = new Mock<IGalleryItem>();
            var mHandler1 = new Mock<IMediaPreprocessor>();
            var mHandler2 = new Mock<IMediaPreprocessor>();
            mHandler1.Setup(h => h.Priority).Returns(1);
            mHandler2.Setup(h => h.Priority).Returns(2);
            mHandler1.Setup(h => h.CanHandle(It.IsAny<IGalleryItem>())).ReturnsAsync(false);
            mHandler2.Setup(h => h.CanHandle(It.IsAny<IGalleryItem>())).ReturnsAsync(true);
            var composite = new CompositeMediaPreprocessor
            {
                mHandler1.Object,
                mHandler2.Object,
            };
            
            Assert.True(await composite.CanHandle(mItem.Object));
        }
        
        [Fact]
        public async Task CompositeHandlerDoesntHandleIfItsEmpty()
        {
            var mItem = new Mock<IGalleryItem>();
            var composite = new CompositeMediaPreprocessor();
            
            Assert.False(await composite.CanHandle(mItem.Object));
        }
        
        [Fact]
        public async Task CompositeHandlerUsesComponentThatCan()
        {
            var mItem = new Mock<IGalleryItem>();
            var mHandler1 = new Mock<IMediaPreprocessor>();
            var mHandler2 = new Mock<IMediaPreprocessor>();
            mHandler1.Setup(h => h.Priority).Returns(1);
            mHandler2.Setup(h => h.Priority).Returns(2);
            mHandler1.Setup(h => h.CanHandle(It.IsAny<IGalleryItem>())).ReturnsAsync(false);
            mHandler2.Setup(h => h.CanHandle(It.IsAny<IGalleryItem>())).ReturnsAsync(true);
            var composite = new CompositeMediaPreprocessor
            {
                mHandler1.Object,
                mHandler2.Object,
            };

            using (var streamIn = new MemoryStream())
            using (var streamOut = new MemoryStream())
            {
                // ReSharper disable once AccessToDisposedClosure
                mHandler2.Setup(h => h.GenerateThumbnail(mItem.Object, streamIn))
                    .ReturnsAsync(streamOut).Verifiable();

                var output = await composite.GenerateThumbnail(mItem.Object, streamIn);

                mHandler2.Verify();
                Assert.Equal(streamOut, output);
            }
        }
        
        [Fact]
        public async Task CompositeHandlerUsesLowestPriorityComponentThatCan()
        {
            var mItem = new Mock<IGalleryItem>();
            var mHandler1 = new Mock<IMediaPreprocessor>();
            var mHandler2 = new Mock<IMediaPreprocessor>();
            var mHandler3 = new Mock<IMediaPreprocessor>();
            mHandler1.Setup(h => h.Priority).Returns(1);
            mHandler2.Setup(h => h.Priority).Returns(2);
            mHandler3.Setup(h => h.Priority).Returns(0);
            mHandler1.Setup(h => h.CanHandle(It.IsAny<IGalleryItem>())).ReturnsAsync(false);
            mHandler2.Setup(h => h.CanHandle(It.IsAny<IGalleryItem>())).ReturnsAsync(true);
            mHandler3.Setup(h => h.CanHandle(It.IsAny<IGalleryItem>())).ReturnsAsync(true);
            var composite = new CompositeMediaPreprocessor
            {
                mHandler1.Object,
                mHandler2.Object,
                mHandler3.Object,
            };

            using (var streamIn = new MemoryStream())
            using (var streamOut = new MemoryStream())
            {
                // ReSharper disable once AccessToDisposedClosure
                mHandler3.Setup(h => h.GenerateThumbnail(mItem.Object, streamIn))
                    .ReturnsAsync(streamOut).Verifiable();

                var output = await composite.GenerateThumbnail(mItem.Object, streamIn);

                mHandler3.Verify();
                Assert.Equal(streamOut, output);
            }
        }
    }
}