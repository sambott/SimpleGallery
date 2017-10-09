using System;
using SimpleGallery.Core.Media;

namespace SimpleGallery.Aws.Media
{
    public abstract class BaseAwsGalleryAlbum : BaseGalleryAlbum, IAwsMediaItem, IEquatable<BaseAwsGalleryAlbum>
    {
        public abstract bool Equals(BaseAwsGalleryAlbum other);
    }
}