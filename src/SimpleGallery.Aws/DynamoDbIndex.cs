using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Logging;
using SimpleGallery.Aws.Model;

namespace SimpleGallery.Aws
{
    public sealed class DynamoDbIndex : IDynamoDbIndex
    {
        private readonly IAmazonDynamoDB _dynamoClient;
        private readonly string _tableName;
        private readonly ILogger _logger;

        public DynamoDbIndex(IAmazonDynamoDB dynamoClient, string tableName, ILogger logger)
        {
            _dynamoClient = dynamoClient;
            _tableName = tableName;
            _logger = logger;
        }

        public IObservable<IAwsIndexItem<IAwsMediaItem>> ScanItems()
        {
            _logger.LogDebug("Creating DynamoDB Scan Observable");
            return Observable.Create<IAwsIndexItem<IAwsMediaItem>>(async (obs, token) =>
            {
                ScanResponse response;
                int page = 0;
                var request = new ScanRequest
                {
                    TableName = _tableName,
                    Limit = 100,
                    ExclusiveStartKey = null,
                };
                do
                {
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    _logger.LogDebug($"Scanning DynamoDB from page {page}");
                    response = await _dynamoClient.ScanAsync(request, token);
                    response.Items.ForEach(i =>
                    {
                        var item = FromDynamoItem(i);
                        _logger.LogDebug($"Received DynamoDB item for {item.Path}");
                        obs.OnNext(item);
                    });
                    request.ExclusiveStartKey = response.LastEvaluatedKey;
                    page++;
                } while (response.LastEvaluatedKey != null && response.LastEvaluatedKey.Count != 0);

                obs.OnCompleted();
            });
        }

        public async Task WriteItem(IAwsIndexItem<IAwsMediaItem> item)
        {
            _logger.LogDebug($"Writing {item.Path} to DynamoDB");
            var dynamoItem = ToDynamoItem(item);
            await _dynamoClient.PutItemAsync(_tableName, dynamoItem);
        }

        public async Task DeleteItem(string itemPath)
        {
            _logger.LogDebug($"Deleting {itemPath} from DynamoDB");
            var dynamoKey = ToDynamoKey(itemPath);
            await _dynamoClient.DeleteItemAsync(_tableName, dynamoKey);
        }

        private static Dictionary<string, AttributeValue> ToDynamoKey(string itemPath)
        {
            return new Dictionary<string, AttributeValue> {
                { "Path", new AttributeValue { S = itemPath } },
            };
        }
        
        private Dictionary<string, AttributeValue> ToDynamoItem(IAwsIndexItem<IAwsMediaItem> item)
        {
            _logger.LogTrace($"Serialising item [{item.Path}] from DynamoDB record");
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
            _logger.LogTrace("Deserialising item from DynamoDB record");
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