using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using SimpleGallery.Core.Model;
using SimpleGallery.Core.Model.MediaPreprocessor;
using SixLabors.ImageSharp;
using Xunit;

namespace SimpleGallery.Core.Tests
{
    public class PhotoHandlerTests
    {
        [Fact]
        public async Task CanOpenJpg()
        {
            var mockMediaItem = new Mock<IGalleryItem>();
            mockMediaItem.Setup(i => i.Path).Returns("any/old/path.JpG");
            
            var photoHandler = new PhotoPreprocessor(new ImageSize(100, 100), 0, Mock.Of<ILogger>());

            Assert.True(await photoHandler.CanHandle(mockMediaItem.Object));
        }
        
        [Fact]
        public async Task CanOpenPng()
        {
            var mockMediaItem = new Mock<IGalleryItem>();
            mockMediaItem.Setup(i => i.Path).Returns("any/old/path.JpG");
            
            var photoHandler = new PhotoPreprocessor(new ImageSize(100, 100), 0, Mock.Of<ILogger>());

            Assert.True(await photoHandler.CanHandle(mockMediaItem.Object));
        }
        
        [Fact]
        public async Task CanOpenBmp()
        {
            var mockMediaItem = new Mock<IGalleryItem>();
            mockMediaItem.Setup(i => i.Path).Returns("any/old/path.BMP");
            
            var photoHandler = new PhotoPreprocessor(new ImageSize(100, 100), 0, Mock.Of<ILogger>());

            Assert.True(await photoHandler.CanHandle(mockMediaItem.Object));
        }
        
        [Fact]
        public async Task CanOpenGif()
        {
            var mockMediaItem = new Mock<IGalleryItem>();
            mockMediaItem.Setup(i => i.Path).Returns("any/old/path.GIf");
            
            var photoHandler = new PhotoPreprocessor(new ImageSize(100, 100), 0, Mock.Of<ILogger>());

            Assert.True(await photoHandler.CanHandle(mockMediaItem.Object));
        }
        
        [Fact]
        public async Task CantOpenOther()
        {
            var mockMediaItem = new Mock<IGalleryItem>();
            mockMediaItem.Setup(i => i.Path).Returns("any/old/path.mp3");
            
            var photoHandler = new PhotoPreprocessor(new ImageSize(100, 100), 0, Mock.Of<ILogger>());

            Assert.False(await photoHandler.CanHandle(mockMediaItem.Object));
        }
        
        [Fact]
        public async Task CanCreateThumbnail()
        {
            var photoHandler = new PhotoPreprocessor(new ImageSize(101, 102), 0, Mock.Of<ILogger>());
            var dummyImage = new Mock<IGalleryItem>().Object;
            using (var imageStream =  File.OpenRead("res/P8050597.JPG"))
            using (var outStream = await photoHandler.GenerateThumbnail(dummyImage, imageStream))
            {
                outStream.Seek(0, 0);
                var thumbnail = Image.Load(outStream);

                Assert.Equal(101, thumbnail.Width);
                Assert.Equal(102, thumbnail.Height);
            }
        }
    }
}