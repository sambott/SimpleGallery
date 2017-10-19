using System.Collections.Generic;
using System.Linq;
using Amazon.S3.Model;

namespace SimpleGallery.Aws.Model
{
    public sealed class AwsGalleryImage : IAwsMediaItem
    {
        private readonly IS3Handler _s3Handler;

        public AwsGalleryImage(S3Object underlying, IS3Handler s3Handler)
        {
            _s3Handler = s3Handler;
            Path = underlying.Key;
            Hash = underlying.ETag;
        }

        public string Path { get; }
        public string Hash { get; }

        public string Name => Path.Split('/').Last();
        public string Url => _s3Handler.UrlForPath(Path);
        public bool IsAlbum => false;
        public ISet<string> ChildPaths { get; } = new HashSet<string>();
        

        private bool Equals(AwsGalleryImage other)
        {
            return  string.Equals(Hash, other.Hash) && string.Equals(Name, other.Name) && string.Equals(Path, other.Path) && IsAlbum == other.IsAlbum;
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
                var hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Path != null ? Path.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Hash != null ? Hash.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ IsAlbum.GetHashCode();
                return hashCode;
            }
        }
    }
}