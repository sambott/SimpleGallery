using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.S3.Model;
using SimpleGallery.Aws.Model;
using SimpleGallery.Core.Model;

namespace SimpleGallery.Aws
{
    public sealed class DynamoDbHandler : IDynamoDbHandler
    {
        private readonly IAmazonDynamoDB _dynamoClient;
        private readonly string _tableName;

        public DynamoDbHandler(IAmazonDynamoDB dynamoClient, string tableName)
        {
            _dynamoClient = dynamoClient;
            _tableName = tableName;
        }

        public IObservable<IAwsIndexItem<IAwsMediaItem>> ScanItems()
        {
            return Observable.Create<IAwsIndexItem<IAwsMediaItem>>(async (obs, token) =>
            {
                ScanResponse response;
                var request = new ScanRequest
                {
                    TableName = _tableName,
                    Limit = 100,
                    ExclusiveStartKey = null,
                };
                do
                {
                    if (token.IsCancellationRequested) break;
                    response = await _dynamoClient.ScanAsync(request, token);
                    response.Items.ForEach(i => obs.OnNext(FromDynamoItem(i)));
                    request.ExclusiveStartKey = response.LastEvaluatedKey;
                } while (response.LastEvaluatedKey != null && response.LastEvaluatedKey.Count != 0);
                obs.OnCompleted();
            });
        }

        public async Task WriteItem(IAwsIndexItem<IAwsMediaItem> item)
        {
            var dynamoItem = ToDynamoItem(item);
            await _dynamoClient.PutItemAsync(_tableName, dynamoItem);
        }

        public async Task DeleteItem(string itemPath)
        {
            var dynamoKey = ToDynamoKey(itemPath);
            await _dynamoClient.DeleteItemAsync(_tableName, dynamoKey);
        }

        private Dictionary<string, AttributeValue> ToDynamoKey(string itemPath)
        {
            return new Dictionary<string, AttributeValue> {
                { "Path", new AttributeValue { S = itemPath } },
            };
        }
        
        private Dictionary<string, AttributeValue> ToDynamoItem(IAwsIndexItem<IAwsMediaItem> item)
        {
            var dynamoItem = ToDynamoKey(item.Path);
            dynamoItem.Add("Name", new AttributeValue {S = item.Name});
            if (item.ChildPaths.Count > 0)
            {
                dynamoItem.Add("ChildPaths", new AttributeValue {SS = item.ChildPaths.ToList()});
            }
            dynamoItem.Add("IsAlbum", new AttributeValue {BOOL = item.IsAlbum});
            dynamoItem.Add("Hash", new AttributeValue {S = item.Hash});
            return dynamoItem;
        }

        private IAwsIndexItem<IAwsMediaItem> FromDynamoItem(Dictionary<string, AttributeValue> dynamoItem)
        {
            var isAlbum = dynamoItem["IsAlbum"].BOOL;
            return new IndexedAwsItem(
                path: dynamoItem["Path"].S,
                name: dynamoItem["Name"].S,
                childPaths: new HashSet<string>(isAlbum ? dynamoItem["ChildPaths"].SS : new List<string>()),
                hash: dynamoItem["Hash"].S,
                isAlbum: isAlbum
            );
        }
    }
}