using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Microsoft.Extensions.Logging;
using Moq;
using SimpleGallery.Aws.Model;
using SimpleGallery.Core.Model;
using SimpleGallery.Core.Tests;
using Xunit;

namespace SimpleGallery.Aws.Tests
{
    public class DynamoDbIndexIntegrationTests
    {
        private const string TableName = "TestDynamoGalleryTable";
        
        [IntegrationFact]
        public async Task CanGetAllItems()
        {
            var dynamoClient = new AmazonDynamoDBClient(RegionEndpoint.EUWest1);
            var handler = new DynamoDbIndex(dynamoClient, TableName, Mock.Of<ILogger>());

            var items = await handler.ScanItems().ToList().ToTask();
            var itemSet = new HashSet<IAwsIndexItem<IAwsMediaItem>>(items);
            
            var expected = new HashSet<IAwsIndexItem<IAwsMediaItem>>(new[]
            {
                new IndexedAwsItem("3 4 5", "345", new HashSet<string>(), "543", false),
                new IndexedAwsItem("2 3 4", "234", new HashSet<string>(new[] {"2","3","4"}), "432", true),
                new IndexedAwsItem("123123", "123", new HashSet<string>(new[] {"1","2","3"}), "112233", true), 
            });
            
            Assert.Equal(expected, itemSet);
        }
    }
}