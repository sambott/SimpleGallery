using System.Collections.Generic;

namespace SimpleGallery.Core.Model.MediaPreprocessor
{
    public class MediaPreprocessorComparer : IComparer<IMediaPreprocessor>
    {
        public int Compare(IMediaPreprocessor x, IMediaPreprocessor y)
        {
            return x.Priority.CompareTo(y.Priority);
        }
    }
}