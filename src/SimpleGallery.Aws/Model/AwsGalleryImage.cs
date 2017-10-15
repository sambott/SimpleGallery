using System.Collections.Generic;
using System.Linq;
using Amazon.S3.Model;

namespace SimpleGallery.Aws.Model
{
    public sealed class AwsGalleryImage : IAwsMediaItem
    {
        private readonly AwsMediaStore _store;
        private readonly S3Object _underlying;

        public AwsGalleryImage(S3Object underlying, AwsMediaStore store)
        {
            _underlying = underlying;
            _store = store;
            Path = underlying.Key;
            Hash = underlying.ETag;
        }

        public string Name => Path.Split('/').Last();
        public string Path { get; }
        public string Url { get; }
        public string Hash { get; }
        
        public bool IsAlbum => false;
        public ISet<string> ChildPaths { get; } = new HashSet<string>();
    }
}