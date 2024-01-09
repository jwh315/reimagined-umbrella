using Amazon.DynamoDBv2;

namespace Hangfire.Storage.DynamoDb
{
    public static class DynamoDbStorageExtensions
    {
        public static IGlobalConfiguration<DynamoDbStorage> UseDynamoDbStorage(
            this IGlobalConfiguration configuration,
            IAmazonDynamoDB client)
        {
            var storage = new DynamoDbStorage(client);

            return configuration.UseStorage(storage);
        }
    }
}