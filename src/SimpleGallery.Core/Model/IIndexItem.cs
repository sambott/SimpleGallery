namespace SimpleGallery.Core.Model
{
    public interface IIndexItem<in TMediaItem> : IMediaItem
        where TMediaItem : IMediaItem
    {
        string ThumbnailUrl { get; }
        bool RequiresUpdate<T>(TMediaItem item);
    }
}