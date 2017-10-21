using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Moq;
using SimpleGallery.Aws.Model;
using Xunit;

namespace SimpleGallery.Aws.Tests
{
    public class DynamoDbHandlerTests
    {
        private const string TableName = "abc123";
        private const string PathName = "123abc";

        [Fact]
        public async Task DeleteItemCallsClient()
        {
            var client = new Mock<IAmazonDynamoDB>();
            client.Setup(c =>
                    c.DeleteItemAsync(
                        TableName,
                        It.Is<Dictionary<string, AttributeValue>>(i =>
                            i["Path"].S == PathName
                        ),
                        It.IsAny<CancellationToken>()
                    )
                )
                .ReturnsAsync(new DeleteItemResponse())
                .Verifiable();
            var handler = new DynamoDbHandler(client.Object, TableName);

            await handler.DeleteItem(PathName);

            client.Verify();
        }

        [Fact]
        public async Task WriteItemCallsClient()
        {
            var item = new IndexedAwsItem("name", PathName, new HashSet<string>(), "hash", false);

            var client = new Mock<IAmazonDynamoDB>();
            client.Setup(c =>
                    c.PutItemAsync(
                        TableName,
                        It.Is<Dictionary<string, AttributeValue>>(i =>
                            i["Path"].S == PathName &&
                            i["Hash"].S == "hash" &&
                            i["Name"].S == "name" &&
                            !i["IsAlbum"].BOOL
                        ),
                        It.IsAny<CancellationToken>()
                    )
                )
                .ReturnsAsync(new PutItemResponse())
                .Verifiable();
            var handler = new DynamoDbHandler(client.Object, TableName);

            await handler.WriteItem(item);

            client.Verify();
        }
    }
}