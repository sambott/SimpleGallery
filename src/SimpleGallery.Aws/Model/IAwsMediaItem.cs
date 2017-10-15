using SimpleGallery.Core.Model;

namespace SimpleGallery.Aws.Model
{
    public interface IAwsMediaItem : IGalleryItem
    {
        string Hash { get; }
    }
}