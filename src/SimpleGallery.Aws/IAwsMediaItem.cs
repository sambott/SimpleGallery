using SimpleGallery.Core.Media;

namespace SimpleGallery.Aws
{
    public interface IAwsMediaItem : IMediaItem
    {
       string Hash { get; } 
    }
}