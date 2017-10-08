using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.S3.Model;

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

        public IObservable<Dictionary<string, AttributeValue>> ScanItems()
        {
            return Observable.Create<Dictionary<string, AttributeValue>>(async (obs, token) =>
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
                    response = await _dynamoClient.ScanAsync(request);
                    response.Items.ForEach(obs.OnNext);
                    request.ExclusiveStartKey = response.LastEvaluatedKey;
                } while (response.LastEvaluatedKey != null && response.LastEvaluatedKey.Count != 0);
                obs.OnCompleted();
            });
        }

        public async Task WriteItem(Dictionary<string, AttributeValue> item)
        {
            await _dynamoClient.PutItemAsync(_tableName, item);
        }

        public async Task DeleteItem(Dictionary<string, AttributeValue> item)
        {
            await _dynamoClient.DeleteItemAsync(_tableName, item);
        }

        private Dictionary<string, AttributeValue> ToDynamoKey(IAwsMediaItem item)
        {
            return new Dictionary<string, AttributeValue> {
                { "Path", new AttributeValue { S = item.Path } },
            };
        }
        
        private Dictionary<string, AttributeValue> ToDynamoItem(IAwsMediaItem item)
        {
            var dynamoItem = ToDynamoKey(item);
            dynamoItem.Add("Hash", new AttributeValue { S = item.Hash });
            dynamoItem.Add("Name", new AttributeValue { S = item.Name });
            dynamoItem.Add("Children", new AttributeValue { SS = item.Children.Select(i => i.Path).ToList() });
            dynamoItem.Add("MediaUrl", new AttributeValue { S = item.MediaUrl });
            dynamoItem.Add("ThubnailUrl", new AttributeValue { S = item.ThumbnailUrl });
            dynamoItem.Add("IsAlbum", new AttributeValue { BOOL = item.IsAlbum });
            return dynamoItem;
        }

        private IAwsMediaItem FromDynamoItem(Dictionary<string, AttributeValue> dynamoItem)
        {
        }
    }
}