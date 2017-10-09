using System;
using SimpleGallery.Core.Media;

namespace SimpleGallery.Aws.Media
{
    public abstract class BaseAwsGalleryImage : BaseGalleryImage, IAwsMediaItem, IEquatable<BaseAwsGalleryImage>
    {
        public abstract string Hash { get; }

        public bool Equals(BaseAwsGalleryImage other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Hash, other.Hash) && string.Equals(Path, other.Path);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is BaseAwsGalleryImage other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Hash != null ? Hash.GetHashCode() : 0) * 397) ^ (Path != null ? Path.GetHashCode() : 0);
            }
        }
    }
}