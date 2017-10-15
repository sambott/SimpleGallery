using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SimpleGallery.Core.Model.MediaHandler
{
    public sealed class CompositeMediaHandler : IMediaHandler, IEnumerable<IMediaHandler>
    {
        private static readonly IComparer<IMediaHandler> HandlerComparer = new MediaHandlerComparer();
        private readonly SortedSet<IMediaHandler> _components = new SortedSet<IMediaHandler>(HandlerComparer);

        public int Priority => 0;

        public bool Add(IMediaHandler item)
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

        private async Task<IMediaHandler> GetHandler(IGalleryItem item)
        {
            foreach (var handler in _components)
            {
                if (await handler.CanHandle(item)) return handler;
            }
            return null;
        }

        public IEnumerator<IMediaHandler> GetEnumerator()
        {
            return _components.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}