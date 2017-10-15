using System;
using System.Threading.Tasks;
using SimpleGallery.Aws.Model;
using SimpleGallery.Core.Model;

namespace SimpleGallery.Aws
{
    public interface IDynamoDbHandler
    {
        IObservable<IAwsIndexItem<IAwsMediaItem>> ScanItems();

        Task WriteItem(IAwsIndexItem<IAwsMediaItem> item);

        Task DeleteItem(string itemPath);
    }
}