using System;
using System.IO;
using System.Threading.Tasks;
using Moq;
using SimpleGallery.Core.Media;
using SimpleGallery.Core.Media.MediaHandler;
using SixLabors.ImageSharp;
using Xunit;

namespace SimpleGallery.Core.Tests
{
    public class PhotoHandlerTests
    {
        [Fact]
        public async Task CanOpenJpg()
        {
            var mockMediaItem = new Mock<IMediaItem>();
            mockMediaItem.Setup(i => i.Path).Returns("any/old/path.JpG");
            
            var photoHandler = new PhotoHandler(new ImageSize(100, 100), 0);

            Assert.True(await photoHandler.CanHandle(mockMediaItem.Object));
        }
        
        [Fact]
        public async Task CanOpenPng()
        {
            var mockMediaItem = new Mock<IMediaItem>();
            mockMediaItem.Setup(i => i.Path).Returns("any/old/path.JpG");
            
            var photoHandler = new PhotoHandler(new ImageSize(100, 100), 0);

            Assert.True(await photoHandler.CanHandle(mockMediaItem.Object));
        }
        
        [Fact]
        public async Task CanOpenBmp()
        {
            var mockMediaItem = new Mock<IMediaItem>();
            mockMediaItem.Setup(i => i.Path).Returns("any/old/path.BMP");
            
            var photoHandler = new PhotoHandler(new ImageSize(100, 100), 0);

            Assert.True(await photoHandler.CanHandle(mockMediaItem.Object));
        }
        
        [Fact]
        public async Task CanOpenGif()
        {
            var mockMediaItem = new Mock<IMediaItem>();
            mockMediaItem.Setup(i => i.Path).Returns("any/old/path.GIf");
            
            var photoHandler = new PhotoHandler(new ImageSize(100, 100), 0);

            Assert.True(await photoHandler.CanHandle(mockMediaItem.Object));
        }
        
        [Fact]
        public async Task CantOpenOther()
        {
            var mockMediaItem = new Mock<IMediaItem>();
            mockMediaItem.Setup(i => i.Path).Returns("any/old/path.mp3");
            
            var photoHandler = new PhotoHandler(new ImageSize(100, 100), 0);

            Assert.False(await photoHandler.CanHandle(mockMediaItem.Object));
        }
        
        [Fact]
        public async Task CanCreateThumbnail()
        {
            using (var outStream = new MemoryStream())
            using (var imageStream = File.OpenRead("res/P8050597.JPG"))
            {
                var mockMediaItem = new Mock<IMediaItem>();
                mockMediaItem.Setup(item => item.GetMedia()).ReturnsAsync(imageStream);
                
                var photoHandler = new PhotoHandler(new ImageSize(100, 100), 0);

                await photoHandler.WriteThumbnail(mockMediaItem.Object, outStream);

                outStream.Seek(0, 0);
                var thumbnail = Image.Load(outStream);

                Assert.Equal(100, thumbnail.Height);
                Assert.Equal(100, thumbnail.Width);
            }
        }
    }
}