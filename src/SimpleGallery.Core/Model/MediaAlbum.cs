using System;
using System.Collections.Generic;

namespace SimpleGallery.Core.Model
{
    public class MediaAlbum : IMediaItem
    {
        public string Name { get; }
        public string Path { get; }
        public bool IsAlbum => true;
        public ISet<string> ChildPaths { get; }

        public MediaAlbum(string name, string path, ISet<string> childPaths)
        {
            Name = name;
            Path = path;
            ChildPaths = childPaths;
        }
    }
}