using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleGallery.Core
{
    public class GalleryBuilder
    {
        private readonly IMediaStore _store;

        public GalleryBuilder(IMediaStore store)
        {
            _store = store;
        }

        public async Task Build()
        {
            var galleryContentItems = await _store.GetAllItems().ConfigureAwait(false);
            var thumbnailItems = await _store.GetAllThumbnails().ConfigureAwait(false);

            var galleryContentPaths = galleryContentItems.Select(item => item.Path).ToList();
            var thumbnailPaths = thumbnailItems.Select(item => item.Path).ToList();

            var pathsToCheck = galleryContentPaths.Intersect(thumbnailPaths);
            var updatesItems = new[] {""}; // TODO check against DB

            var added = galleryContent.Except(thumbnails).ToList();
            var removed = thumbnails.Except(galleryContent).ToList();

            await Task.WhenAll(removed.Select(_store.RemoveThumbnail));
            await Task.WhenAll(removed.Select(_store.RemoveIndex));

            await Task.WhenAll(added.Select(_store.UpdateThumbnail));
            await Task.WhenAll(added.Select(_store.UpdateIndex));
            
            
        }

        public async Task CheckThumbnailAndDataConsistent()
        {
            //TODO run this periodically to stop DB and thubnails gettting out of check
        }
    }
}