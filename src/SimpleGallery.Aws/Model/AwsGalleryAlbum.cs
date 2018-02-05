using System;
using System.Collections.Generic;

namespace SimpleGallery.Aws.Model
{
    public sealed class AwsGalleryAlbum : IAwsMediaItem, IEquatable<AwsGalleryAlbum>
    {
        public AwsGalleryAlbum(string name, string path, string url, ISet<string> childPaths)
        {
            Name = name;
            Path = path;
            Url = url;
            ChildPaths = childPaths;
        }

        public string Name { get; }
        public string Path { get; }
        public string Url { get; }
        public ISet<string> ChildPaths { get; }
        public bool IsAlbum => true;
        public string Hash { get; } = ""; // TODO should generate for albums??
        

        public bool Equals(AwsGalleryAlbum other)
        {
            return  string.Equals(Hash, other.Hash) && string.Equals(Name, other.Name) && string.Equals(Path, other.Path) && ChildPaths.SetEquals(other.ChildPaths) && IsAlbum == other.IsAlbum;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((AwsGalleryAlbum) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Path != null ? Path.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Hash != null ? Hash.GetHashCode() : 0);
                foreach (var path in ChildPaths)
                {
                    hashCode = (hashCode * 397) ^ (path != null ? path.GetHashCode() : 0);
                }
                hashCode = (hashCode * 397) ^ IsAlbum.GetHashCode();
                return hashCode;
            }
        }
    }
}