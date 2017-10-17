using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using SimpleGallery.Aws.Model;
using SimpleGallery.Core.Tests;
using Xunit;

namespace SimpleGallery.Aws.Tests
{
    public class DynamoDbHandlerIntegrationTests
    {
        private const string TableName = "TestDynamoGalleryTable";
        
        [IntegrationFact]
        public async Task CanGetAllItems()
        {
            var dynamoClient = new AmazonDynamoDBClient(RegionEndpoint.EUWest1);
            var handler = new DynamoDbHandler(dynamoClient, TableName);

            var items = await handler.ScanItems().ToList().ToTask();
            
            Assert.Equal(3, items.Count);
        }
    }
}