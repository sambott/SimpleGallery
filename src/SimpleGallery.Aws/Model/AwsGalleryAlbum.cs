using System.Collections.Generic;
using SimpleGallery.Core.Model;

namespace SimpleGallery.Aws.Model
{
    public sealed class AwsGalleryAlbum : IAwsMediaItem
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
        public string Hash { get; } = "";
    }
}