using SimpleGallery.Core.Model;

namespace SimpleGallery.Aws.Model
{
    public interface IAwsIndexItem<in TMediaItem> : IIndexItem<TMediaItem>
        where TMediaItem : IAwsMediaItem
    {
        string Hash { get; }
    }
}