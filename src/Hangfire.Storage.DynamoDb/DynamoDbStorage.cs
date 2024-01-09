using Amazon.DynamoDBv2;

namespace Hangfire.Storage.DynamoDb
{
    public sealed class DynamoDbStorage: JobStorage
    {
        private readonly IAmazonDynamoDB _client;

        public DynamoDbStorage(IAmazonDynamoDB client)
        {
            _client = client;
        }
        
        public override IMonitoringApi GetMonitoringApi()
        {
            throw new System.NotImplementedException();
        }

        public override IStorageConnection GetConnection() => new DynamoDbConnection(_client);
    }
}