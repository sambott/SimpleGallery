using System.Threading.Tasks;
using Moq;
using SimpleGallery.Core.Model;

namespace SimpleGallery.Core.Tests
{
    public class GalleryViewerTests
    {
        public async Task TestTest()
        {
            // necessary?
            
            const char seperator = '/';
            var store = new Mock<IMediaStore<IGalleryItem, IGalleryItem, IIndexItem<IGalleryItem>>>();
            var indexItems = await store.Object.GetAllIndexItems();
            foreach (var item in indexItems)
            {
                var pathParts = item.Path.Split(seperator);
            }
        }
    }
}