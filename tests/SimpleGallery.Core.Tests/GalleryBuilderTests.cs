using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using SimpleGallery.Core.Model;
using SimpleGallery.Core.Model.MediaPreprocessor;
using Xunit;

namespace SimpleGallery.Core.Tests
{
    public class GalleryBuilderTests
    {
        private IGalleryItem GenerateMediaItem(string path)
        {
            var item = new Mock<IGalleryItem>();
            item.Setup(i => i.Path).Returns(path);
            item.Setup(i => i.Name).Returns(path.Split('/').Last());
            return item.Object;
        }
        
        private IIndexItem<IGalleryItem> GenerateIndexItem(string path)
        {
            var item = new Mock<IIndexItem<IGalleryItem>>();
            item.Setup(i => i.Path).Returns(path);
            item.Setup(i => i.Name).Returns(path.Split('/').Last());
            return item.Object;
        }

        [Fact]
        public async Task BringsIndexAndThumbnailsInCheck()
        {
            var thumbnailItems = new List<IGalleryItem>
            {
                GenerateMediaItem("test/item1"),
                GenerateMediaItem("test/item2"),
                GenerateMediaItem("item4"),
                GenerateMediaItem("item3")
            };
            var indexItems = new List<IIndexItem<IGalleryItem>>
            {
                GenerateIndexItem("test/item1"),
                GenerateIndexItem("test/item2"),
                GenerateIndexItem("test/item3"),
                GenerateIndexItem("item3")
            };
            var mockMediaStore = new Mock<IMediaStore<IGalleryItem, IGalleryItem, IIndexItem<IGalleryItem>>>();
            mockMediaStore.Setup(ms => ms.GetAllThumbnails()).ReturnsAsync(thumbnailItems);
            mockMediaStore.Setup(ms => ms.GetAllIndexItems()).ReturnsAsync(indexItems);
            mockMediaStore.Setup(ms => ms.GetAllItems()).ReturnsAsync(new List<IGalleryItem>());

            var mediaHandler = new Mock<IMediaPreprocessor>();
            mediaHandler.Setup(h =>
                h.CanHandle(Match.Create<IGalleryItem>(_ => true))
            ).ReturnsAsync(true);
            
            var builder = new GalleryBuilder<IGalleryItem, IGalleryItem, IIndexItem<IGalleryItem>>(mockMediaStore.Object, mediaHandler.Object, Mock.Of<ILogger>());
            await builder.LoadItemSources();

            mockMediaStore.Setup(ms =>
                ms.RemoveThumbnail("item4")
            ).Returns(Task.CompletedTask).Verifiable();

            mockMediaStore.Setup(ms =>
                ms.RemoveIndex(
                    "test/item3"
                )
            ).Returns(Task.CompletedTask).Verifiable();

            await builder.MakeThumbnailAndIndexConsistent();

            mockMediaStore.Verify();
        }

        [Fact]
        public async Task FindsDelta()
        {
            var galleryContentItems = new List<IGalleryItem>
            {
                GenerateMediaItem("test/item1"),
                GenerateMediaItem("test/item2"),
                GenerateMediaItem("item4"),
                GenerateMediaItem("item3")
            };
            var indexItems = new List<IIndexItem<IGalleryItem>>
            {
                GenerateIndexItem("test/item1"),
                GenerateIndexItem("test/item2"),
                GenerateIndexItem("test/item3"),
                GenerateIndexItem("item3")
            };
            var mockMediaStore = new Mock<IMediaStore<IGalleryItem, IGalleryItem, IIndexItem<IGalleryItem>>>();
            mockMediaStore.Setup(ms => ms.GetAllItems()).ReturnsAsync(galleryContentItems);
            mockMediaStore.Setup(ms => ms.GetAllIndexItems()).ReturnsAsync(indexItems);
            mockMediaStore.Setup(ms => ms.GetAllThumbnails()).ReturnsAsync(new List<IGalleryItem>());

            var mediaHandler = new Mock<IMediaPreprocessor>();
            mediaHandler.Setup(h =>
                h.CanHandle(Match.Create<IGalleryItem>(_ => true))
            ).ReturnsAsync(true);
            
            var builder = new GalleryBuilder<IGalleryItem, IGalleryItem, IIndexItem<IGalleryItem>>(mockMediaStore.Object, mediaHandler.Object, Mock.Of<ILogger>());
            await builder.LoadItemSources();

            var (added, removed, remaining) = builder.GetAddedRemovedRemaining();

            var expectedAdded = new HashSet<string>
            {
                "item4"
            };
            var expectedRemoved = new HashSet<string>
            {
                "test/item3"
            };
            var expectedRemaining = new HashSet<string>
            {
                "test/item1",
                "test/item2",
                "item3"
            };

            Assert.Equal(expectedAdded, new HashSet<string>(added.Select(i => i.Path)));
            Assert.Equal(expectedRemoved, new HashSet<string>(removed.Select(i => i.Path)));
            Assert.Equal(expectedRemaining, new HashSet<string>(remaining.Select(i => i.Path)));
        }

        //TODO test GetUpdated && Build()
    }
}