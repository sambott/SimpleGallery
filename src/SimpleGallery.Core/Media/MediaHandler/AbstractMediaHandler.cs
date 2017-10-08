using System.IO;
using System.Threading.Tasks;

namespace SimpleGallery.Core.Media.MediaHandler
{
    public abstract class AbstractMediaHandler : IMediaHandler
    {
        public abstract Task<bool> CanHandle(IMediaItem item);
        public abstract Task WriteThumbnail(IMediaItem item, Stream output);
        public abstract int Priority { get; }

        public int CompareTo(IMediaHandler other)
        {
            return Priority.CompareTo(other.Priority);
        }
    }
}