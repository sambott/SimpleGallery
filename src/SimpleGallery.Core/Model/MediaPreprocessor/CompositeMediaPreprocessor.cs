using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SimpleGallery.Core.Model.MediaPreprocessor
{
    public sealed class CompositeMediaPreprocessor : IMediaPreprocessor, IEnumerable<IMediaPreprocessor>
    {
        private static readonly IComparer<IMediaPreprocessor> HandlerComparer = new MediaPreprocessorComparer();
        private readonly SortedSet<IMediaPreprocessor> _components = new SortedSet<IMediaPreprocessor>(HandlerComparer);

        public int Priority => 0;

        public bool Add(IMediaPreprocessor item)
        {
            return _components.Add(item);
        }

        public async Task<bool> CanHandle(IGalleryItem item)
        {
            return (await GetHandler(item)) != null;
        }

        public async Task<Stream> GenerateThumbnail(IGalleryItem item, Stream input)
        {
            var handler = await GetHandler(item);
            return await handler.GenerateThumbnail(item, input);
        }

        private async Task<IMediaPreprocessor> GetHandler(IGalleryItem item)
        {
            foreach (var handler in _components)
            {
                if (await handler.CanHandle(item)) return handler;
            }
            return null;
        }

        public IEnumerator<IMediaPreprocessor> GetEnumerator()
        {
            return _components.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}