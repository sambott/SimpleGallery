using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3.Model;
using SimpleGallery.Core;

namespace SimpleGallery.Aws
{
    public sealed class AwsGalleryImage : GalleryImage
    {
        public bool Equals(AwsGalleryImage other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(_hash, other._hash) && string.Equals(Path, other.Path);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is AwsGalleryImage && Equals((AwsGalleryImage) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_hash != null ? _hash.GetHashCode() : 0) * 397) ^ (Path != null ? Path.GetHashCode() : 0);
            }
        }

        private readonly string _hash;
        private readonly AwsMediaStore _store;
        private readonly S3Object _underlying;

        public AwsGalleryImage(S3Object underlying, AwsMediaStore store)
        {
            _underlying = underlying;
            _store = store;
            Path = underlying.Key;
            _hash = underlying.ETag;
        }

        public override string Name => Path.Split('/').Last();
        public override string Path { get; }
        public override string MediaUrl { get; }
        public override string ThumbnailUrl { get; }

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