using System;
using SimpleGallery.Core.Media;

namespace SimpleGallery.Aws
{
    public abstract class BaseAwsGalleryAlbum : BaseGalleryAlbum, IEquatable<BaseAwsGalleryAlbum>
    {
        public abstract bool Equals(BaseAwsGalleryAlbum other);
    }
}