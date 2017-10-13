using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3.Model;

namespace SimpleGallery.Aws.Media
{
    public sealed class AwsGalleryImage : BaseAwsGalleryImage
    {
        private readonly AwsMediaStore _store;
        private readonly S3Object _underlying;

        public AwsGalleryImage(S3Object underlying, AwsMediaStore store)
        {
            _underlying = underlying;
            _store = store;
            Path = underlying.Key;
            Hash = underlying.ETag;
        }

        public override string Name => Path.Split('/').Last();
        public override string Path { get; }
        public override string Hash { get; }
        public override string MediaUrl { get; }
        public override string ThumbnailUrl { get; }


        // TODO move this into the store
        public override Task<Stream> GetMedia()
        {
            throw new NotImplementedException();
        }

        public override Task<Stream> GetThumbnail()
        {
            throw new NotImplementedException();
        }
    }
}