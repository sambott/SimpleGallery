namespace SimpleGallery.Core.Model
{
    public interface IIndexItem<in TMediaItem> : IMediaItem
        where TMediaItem : IGalleryItem
    {
        bool RequiresUpdate(TMediaItem item);
    }
}