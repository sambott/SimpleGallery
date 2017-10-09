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
using SimpleGallery.Aws.Media;

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

        private Dictionary<string, AttributeValue> ToDynamoKey(BaseAwsGalleryImage item)
        {
            return new Dictionary<string, AttributeValue> {
                { "Path", new AttributeValue { S = item.Path } },
            };
        }
        
        private Dictionary<string, AttributeValue> ToDynamoItem(BaseAwsGalleryImage item)
        {
            // TODO consider a serialisable attribute or interface
            var dynamoItem = ToDynamoKey(item);
            dynamoItem.Add("Hash", new AttributeValue { S = item.Hash });
            dynamoItem.Add("Name", new AttributeValue { S = item.Name });
            dynamoItem.Add("ChildPaths", new AttributeValue { SS = item.ChildPaths.ToList() });
            dynamoItem.Add("MediaUrl", new AttributeValue { S = item.MediaUrl });
            dynamoItem.Add("ThubnailUrl", new AttributeValue { S = item.ThumbnailUrl });
            dynamoItem.Add("IsAlbum", new AttributeValue { BOOL = item.IsAlbum });
            return dynamoItem;
        }

        private IAwsMediaItem FromDynamoItem(Dictionary<string, AttributeValue> dynamoItem)
        {
            if (dynamoItem["IsAlbum"].BOOL)
            {
                return new IndexedGalleryAlbum(
                    name: dynamoItem["Name"].S,
                    path: dynamoItem["Path"].S,
                    mediaUrl: dynamoItem["MediaUrl"].S,
                    thumbnailUrl: dynamoItem["ThubnailUrl"].S,
                    childPaths: new HashSet<string>(dynamoItem["ChildPaths"].SS)
                  );
            }
            else
            {
                return new IndexedGalleryImage(
                    name: dynamoItem["Name"].S,
                    path: dynamoItem["Path"].S,
                    mediaUrl: dynamoItem["MediaUrl"].S,
                    thumbnailUrl: dynamoItem["ThubnailUrl"].S,
                    hash: dynamoItem["Hash"].S
                );
            }
        }
    }
}