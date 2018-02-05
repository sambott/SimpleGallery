namespace SimpleGallery.Api.Models
{
    public class AppOptions
    {
        
        public GalleryBuilderOptions GalleryBuilder { get; set; }
        
    }

    public class GalleryBuilderOptions
    {
        public int RebuildIntervalSeconds { get; set; }
    }
}