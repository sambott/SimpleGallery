using System;
using System.Threading.Tasks;
using SixLabors.ImageSharp;

namespace SimpleGallery.Core
{
    public interface IMediaStore
    {
        IGalleryAlbum GetRootAlbum();

        Task StoreThumnail(string path, IMediaItem item);
    }
}