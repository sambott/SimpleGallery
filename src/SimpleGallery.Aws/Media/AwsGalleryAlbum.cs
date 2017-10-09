using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SimpleGallery.Core.Media;

namespace SimpleGallery.Aws.Media
{
    public class AwsGalleryAlbum : BaseAwsGalleryAlbum
    {
        public override string Name { get; }
        public override string Path { get; }
        public override string MediaUrl { get; }
        public override string ThumbnailUrl { get; }
        public override ISet<string> ChildPaths { get; }
        public override Task<Stream> GetMedia()
        {
            throw new System.NotImplementedException();
        }

        public override Task<Stream> GetThumbnail()
        {
            throw new System.NotImplementedException();
        }

        public override bool Equals(BaseAwsGalleryAlbum other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Path, other.Path);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AwsGalleryAlbum) obj);
        }

        public override int GetHashCode()
        {
            return (Path != null ? Path.GetHashCode() : 0);
        }
    }
}