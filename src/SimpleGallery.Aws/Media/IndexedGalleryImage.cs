using System.IO;
using System.Threading.Tasks;

namespace SimpleGallery.Aws.Media
{
    public class IndexedGalleryImage : BaseAwsGalleryImage
    {
        public IndexedGalleryImage(string name, string path, string hash, string mediaUrl, string thumbnailUrl)
        {
            Name = name;
            Path = path;
            Hash = hash;
            MediaUrl = mediaUrl;
            ThumbnailUrl = thumbnailUrl;
        }

        public override string Name { get; }
        public override string Path { get; }
        public override string Hash { get; }
        public override string MediaUrl { get; }
        public override string ThumbnailUrl { get; }


        public override Task<Stream> GetMedia()
        {
            throw new System.NotImplementedException();
        }

        public override Task<Stream> GetThumbnail()
        {
            throw new System.NotImplementedException();
        }
    }
}