using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace SimpleGallery.Core.Tests
{
    public class GalleryBuilderTests
    {

        private IMediaItem GenerateMediaItem(string path)
        {
            var item = new Mock<IMediaItem>();
            item.Setup(i => i.Path).Returns(path);
            item.Setup(i => i.Name).Returns(path.Split('/').Last());
            return item.Object;
        }

        [Fact]
        public async Task BringsIndexAndThumbnailsInCheck()
        {
            var thumbnailItems = new List<IMediaItem>
            {
                GenerateMediaItem("test/item1"),
                GenerateMediaItem("test/item2"),
                GenerateMediaItem("item4"),
                GenerateMediaItem("item3"),
            };
            var indexItems = new List<IMediaItem>
            {
                GenerateMediaItem("test/item1"),
                GenerateMediaItem("test/item2"),
                GenerateMediaItem("test/item3"),
                GenerateMediaItem("item3"),
            };
            var mockMediaStore = new Mock<IMediaStore>();
            mockMediaStore.Setup(ms => ms.GetAllThumbnails()).ReturnsAsync(thumbnailItems);
            mockMediaStore.Setup(ms => ms.GetIndexItems()).ReturnsAsync(indexItems);

            var builder = new GalleryBuilder(mockMediaStore.Object);

            mockMediaStore.Setup(ms =>
                ms.RemoveThumbnail(
                    Match.Create<IMediaItem>(i => i.Path == "item4")       
                )
            ).Verifiable();
            
            mockMediaStore.Setup(ms =>
                ms.RemoveIndex(
                    Match.Create<IMediaItem>(i => i.Path == "test/item3")       
                )
            ).Verifiable();
                  
            await builder.CheckThumbnailAndIndexConsistent();
            
            mockMediaStore.Verify();
        }

        [Fact]
        public async Task FindsDelta()
        {
            var galleryContentItems = new List<IMediaItem>
                {
                    GenerateMediaItem("test/item1"),
                    GenerateMediaItem("test/item2"),
                    GenerateMediaItem("item4"),
                    GenerateMediaItem("item3"),
                };
            var indexItems = new List<IMediaItem>
                {
                    GenerateMediaItem("test/item1"),
                    GenerateMediaItem("test/item2"),
                    GenerateMediaItem("test/item3"),
                    GenerateMediaItem("item3"),
                };
            var mockMediaStore = new Mock<IMediaStore>();
            mockMediaStore.Setup(ms => ms.GetAllItems()).ReturnsAsync(galleryContentItems);
            mockMediaStore.Setup(ms => ms.GetIndexItems()).ReturnsAsync(indexItems);

            var builder = new GalleryBuilder(mockMediaStore.Object);
            
            var (added, removed, remaining) = await builder.GetAddedRemovedRemaining();
            
            var expectedAdded = new HashSet<string>
            {
               "item4",
            };
            var expectedRemoved = new HashSet<string>
            {
                "test/item3",
            };
            var expectedRemaining = new HashSet<string>
            {
                "test/item1",
                "test/item2",
                "item3",
            };

            Assert.Equal(expectedAdded, new HashSet<string>(added.Select(i => i.Path)));
            Assert.Equal(expectedRemoved, new HashSet<string>(removed.Select(i => i.Path)));
            Assert.Equal(expectedRemaining, new HashSet<string>(remaining.Select(i => i.Path)));
        }
    }
}