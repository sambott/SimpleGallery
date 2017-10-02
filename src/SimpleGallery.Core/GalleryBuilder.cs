using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
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

        public async Task<(IEnumerable<IMediaItem>, IEnumerable<IMediaItem>, IEnumerable<IMediaItem>)> GetAddedRemovedRemaining()
        {
            var galleryContentItems = await _store.GetAllItems().ConfigureAwait(false);
            var thumbnailItems = await _store.GetAllThumbnails().ConfigureAwait(false);

            var galleryContentPathDict = galleryContentItems.ToDictionary(item => item.Path);
            var thumbnailPathDict = thumbnailItems.ToDictionary(item => item.Path);

            var addedPaths = galleryContentPathDict.Keys.Except(thumbnailPathDict.Keys).ToList();
            var removedPaths = thumbnailPathDict.Keys.Except(galleryContentPathDict.Keys);
            var remainingPaths = galleryContentPathDict.Keys.Except(addedPaths);

            var added = addedPaths.Select(path => galleryContentPathDict[path]);
            var removed = removedPaths.Select(path => thumbnailPathDict[path]);
            var remaining = remainingPaths.Select(path => galleryContentPathDict[path]);
            
            return (added, removed, remaining);
        }

        public async Task Build()
        {
            var updatesItems = new[] {""}; // TODO check against DB

            

//            await Task.WhenAll(removed.Select(_store.RemoveThumbnail));
//            await Task.WhenAll(removed.Select(_store.RemoveIndex));
//
//            await Task.WhenAll(added.Select(_store.UpdateThumbnail));
//            await Task.WhenAll(added.Select(_store.UpdateIndex));
            
            
        }

        public async Task CheckThumbnailAndDataConsistent()
        {
            //TODO run this periodically to stop DB and thubnails gettting out of check
        }
    }
}