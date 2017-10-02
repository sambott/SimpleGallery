using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace SimpleGallery.Core.Tests
{
    public class GalleryBuilderTests
    {
        private readonly IEnumerable<IMediaItem> _sampleList;
        
        public GalleryBuilderTests()
        {
            _sampleList = new List<IMediaItem>
            {
                GenerateMediaItem("test/item1"),
                GenerateMediaItem("test/item2"),
                GenerateMediaItem("item3"),
            };
        }

        private IMediaItem GenerateMediaItem(string path)
        {
            return new Mock<IMediaItem>()
                .SetupProperty(i => i.Path, path)
                .SetupProperty(i => i.Name, path.Split('/').Last())
                .Object;
        }

        [Fact]
        public Task BuildFindsDelta()
        {
            var mockMediaStore = new Mock<IMediaStore>();
            mockMediaStore.Setup(ms => ms.GetAllItems()).ReturnsAsync(_sampleList);
            
            mockMediaStore.
        }
    }
}