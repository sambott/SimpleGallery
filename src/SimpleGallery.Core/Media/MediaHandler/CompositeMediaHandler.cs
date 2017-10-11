using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SimpleGallery.Core.Media.MediaHandler
{
    public sealed class CompositeMediaHandler : AbstractMediaHandler
    {
        private readonly SortedSet<IMediaHandler> _components = new SortedSet<IMediaHandler>();

        public override int Priority => 0;

        public bool Add(IMediaHandler item)
        {
            return _components.Add(item);
        }

        public override async Task<bool> CanHandle(IMediaItem item)
        {
            return (await GetHandler(item)) != null;
        }

        public override async Task WriteThumbnail(IMediaItem item, Stream output)
        {
            var handler = await GetHandler(item);
            await handler.WriteThumbnail(item, output);
        }

        private async Task<IMediaHandler> GetHandler(IMediaItem item)
        {
            foreach (var handler in _components)
            {
                if (await handler.CanHandle(item)) return handler;
            }
            return null;
        }
    }
}