using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3.Model;
using SimpleGallery.Core;

namespace SimpleGallery.Aws
{
    public sealed class AwsGalleryImage : GalleryImage
    {
        private readonly S3Object _underlying;
        private readonly AwsMediaStore _store;
        private readonly string _bucketPrefix;
        private readonly string _hash;

        private bool Equals(AwsGalleryImage other)
        {
            return string.Equals(_hash, other._hash) && string.Equals(Path, other.Path);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AwsGalleryImage) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_hash != null ? _hash.GetHashCode() : 0) * 397) ^ (Path != null ? Path.GetHashCode() : 0);
            }
        }

        public AwsGalleryImage((int, int) thumbnailSize, S3Object underlying, AwsMediaStore store) : base(thumbnailSize)
        {
            _underlying = underlying;
            _store = store;
            Path = underlying.Key.Remove(0, _store.BucketPrefix.Length);
            _hash = underlying.ETag;
        }

        public override string Name => Path.Split('/').Last();
        public override string Path { get; }
        public override string MediaUrl { get; }
        public override string ThumbnailUrl { get; }

        public override Task<Stream> GetMedia()
        {
            throw new System.NotImplementedException();
        }

        public override Task<Stream> GetThumbnail()
        {
            throw new System.NotImplementedException();
        }
    }
}