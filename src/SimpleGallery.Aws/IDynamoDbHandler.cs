using System;
using System.Threading.Tasks;
using SimpleGallery.Aws.Media;
using SimpleGallery.Core.Media;

namespace SimpleGallery.Aws
{
    public interface IDynamoDbHandler
    {
        IObservable<IAwsMediaItem> ScanItems();

        Task WriteItem(IMediaItem item);

        Task DeleteItem(IMediaItem item);
    }
}