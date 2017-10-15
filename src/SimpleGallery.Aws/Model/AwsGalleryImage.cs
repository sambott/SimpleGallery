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
    }
}