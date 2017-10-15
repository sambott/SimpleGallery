using SimpleGallery.Core.Model;

namespace SimpleGallery.Aws.Model
{
    public interface IAwsMediaItem : IMediaItem
    {
        string Hash { get; }
    }
}