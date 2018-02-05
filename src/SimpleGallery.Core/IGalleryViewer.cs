using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleGallery.Core.Model;

namespace SimpleGallery.Core
{
    public interface IGalleryViewer
    {
        Task<IEnumerable<IMediaItem>> ItemsInAlbum(string album);
    }
}