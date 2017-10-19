using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SimpleGallery.Core.Model;

namespace SimpleGallery.Aws.Model
{
    public sealed class IndexedAwsItem : IAwsIndexItem<IAwsMediaItem>
    {

        public IndexedAwsItem(string name, string path, ISet<string> childPaths, string hash, bool isAlbum)
        {
            Name = name;
            Path = path;
            ChildPaths = childPaths;
            Hash = hash;
            IsAlbum = isAlbum;
        }

        public string Name { get; }
        public string Path { get; }
        public string Hash { get; }
        public ISet<string> ChildPaths { get; }
        public bool IsAlbum { get; }

        public bool RequiresUpdate(IAwsMediaItem item) => !UpToDateWith(item);
        
        private bool UpToDateWith(IAwsMediaItem item)
        {
            if (ReferenceEquals(null, item)) return false;
            return Equals((IMediaItem) item) && string.Equals(Hash, item.Hash);
        }
        
        private bool Equals(IMediaItem other)
        {
            return string.Equals(Name, other.Name) && string.Equals(Path, other.Path) && ChildPaths.SetEquals(other.ChildPaths) && IsAlbum == other.IsAlbum;
        }

        private bool Equals(IndexedAwsItem other)
        {
            return Equals((IMediaItem) other) && string.Equals(Hash, other.Hash);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IndexedAwsItem) obj);
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