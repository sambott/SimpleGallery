using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;

namespace SimpleGallery.Aws
{
    public interface IDynamoDbHandler
    {
        IObservable<Dictionary<string, AttributeValue>> ScanItems();

        Task WriteItem(Dictionary<string, AttributeValue> item);

        Task DeleteItem(Dictionary<string, AttributeValue> item);
    }
}