using System.Collections.Generic;

namespace SimpleGallery.Core.Model.MediaHandler
{
    public class MediaHandlerComparer : IComparer<IMediaHandler>
    {
        public int Compare(IMediaHandler x, IMediaHandler y)
        {
            return x.Priority.CompareTo(y.Priority);
        }
    }
}