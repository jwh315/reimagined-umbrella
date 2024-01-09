using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Newtonsoft.Json;

namespace Hangfire.Storage.DynamoDb.Helper
{
    public class DynamoRecord
    {
        public string Id { get; set; }

        public string Value { get; set; }
    }
    
    public class DynamoDbHelper
    {
        private readonly IDynamoDBContext _context;

        public DynamoDbHelper(IDynamoDBContext context)
        {
            _context = context;
        }
        
        public Task Save<T>(string key, T value)
        {
            var record = new DynamoRecord()
            {
                Id = key,
                Value = JsonConvert.SerializeObject(value)
            };

            return _context.SaveAsync(
                record,
                new DynamoDBOperationConfig { OverrideTableName = "HangfireJobStorage" }
            );
        }

        public async Task<T> Fetch<T>(string key)
        {
            var result = await _context.LoadAsync<DynamoRecord>(
                key,
                new DynamoDBOperationConfig { OverrideTableName = "HangfireJobStorage" }
            );

            return result != null ? JsonConvert.DeserializeObject<T>(result.Value) : default;
        }
        
        public async Task Delete<T>(string key, T value)
        {
            var record = new DynamoRecord
            {
                Id = key,
                Value = JsonConvert.SerializeObject(value)
            };
            
            await _context.DeleteAsync(
                record,
                new DynamoDBOperationConfig { OverrideTableName = "HangfireJobStorage" }
            );
        }

        public TransactWrite<DynamoRecord> CreateWriteTransaction()
        {
            return _context.CreateTransactWrite<DynamoRecord>(
                new DynamoDBOperationConfig { OverrideTableName = "HangfireJobStorage" }
            );
        }
    }
}