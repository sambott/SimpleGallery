using System;
using System.Collections.Generic;
using SimpleGallery.Core.Model;

namespace SimpleGallery.Aws.Model
{
    public class IndexedAwsItem : IAwsIndexItem<IAwsMediaItem>
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

        public bool RequiresUpdate<T>(IAwsMediaItem item)
        {
            throw new NotImplementedException();
        }
    }
}