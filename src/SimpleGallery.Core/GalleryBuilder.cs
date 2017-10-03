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

        public async Task CheckThumbnailAndIndexConsistent()
        {
            var thumbnailItems = await _store.GetAllThumbnails().ConfigureAwait(false);
            var indexItems = await _store.GetIndexItems().ConfigureAwait(false);

            var thumbnailPathDict = thumbnailItems.ToDictionary(item => item.Path);
            var indexPathDict = indexItems.ToDictionary(item => item.Path);

            var thumbnailDeltaPaths = thumbnailPathDict.Keys.Except(indexPathDict.Keys);
            var indexDeltaPaths = indexPathDict.Keys.Except(thumbnailPathDict.Keys).ToList();

            thumbnailDeltaPaths
                .Select(path => thumbnailPathDict[path])
                .ToList()
                .ForEach(async item => await _store.RemoveThumbnail(item));
            indexDeltaPaths
                .Select(path => indexPathDict[path])
                .ToList()
                .ForEach(async item => await _store.RemoveIndex(item));
        }

        public async Task<(IEnumerable<IMediaItem>, IEnumerable<IMediaItem>, IEnumerable<IMediaItem>)>
            GetAddedRemovedRemaining()
        {
            var galleryContentItems = await _store.GetAllItems().ConfigureAwait(false);
            var indexItems = await _store.GetIndexItems().ConfigureAwait(false);

            var galleryContentPathDict = galleryContentItems.ToDictionary(item => item.Path);
            var indexPathDict = indexItems.ToDictionary(item => item.Path);

            var addedPaths = galleryContentPathDict.Keys.Except(indexPathDict.Keys).ToList();
            var removedPaths = indexPathDict.Keys.Except(galleryContentPathDict.Keys);
            var remainingPaths = galleryContentPathDict.Keys.Except(addedPaths);

            var added = addedPaths.Select(path => galleryContentPathDict[path]);
            var removed = removedPaths.Select(path => indexPathDict[path]);
            var remaining = remainingPaths.Select(path => galleryContentPathDict[path]);

            return (added, removed, remaining);
        }

        public async Task Build()
        {
            //make thumbnails and index consistent

            //check for added/removed

            //check for updated

            //remove removed and updated

            //add added and updated
        }
    }
}