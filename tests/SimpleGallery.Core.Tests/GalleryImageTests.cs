using System;
using System.IO;
using SixLabors.ImageSharp;
using Xunit;

namespace SimpleGallery.Core.Tests
{
    public class GalleryImageTests
    {
        private class TestGalleryImage : GalleryImage
        {
            private readonly Stream _mediaStream;

            public TestGalleryImage((int, int) thumbnailSize, Stream mediaStream) : base(thumbnailSize)
            {
                _mediaStream = mediaStream;
            }

            public override string Name => "";
            public override string Path => "";
            public override string MediaUrl => "";
            public override string ThumbnailUrl => "";
            public override Stream GetMedia() => _mediaStream;

            public override Stream GetThumbnail()
            {
                throw new NotImplementedException();
            }
        }
        
        [Fact]
        public void CanCreateThumbnail()
        {
            using (var outStream = new MemoryStream())
            using (var imageStream = File.OpenRead("res/P8050597.JPG"))
            {
                var galleryImage = new TestGalleryImage((100,100), imageStream);
                
                galleryImage.GenerateThumbnail(outStream);

                outStream.Seek(0, 0);
                var thumbnail = Image.Load(outStream);
                
                Assert.Equal(100, thumbnail.Height);
                Assert.Equal(100, thumbnail.Width);
            }
        }
    }
}