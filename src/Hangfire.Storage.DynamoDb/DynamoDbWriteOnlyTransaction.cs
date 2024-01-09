using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Hangfire.States;

namespace Hangfire.Storage.DynamoDb
{
    public class DynamoDbWriteOnlyTransaction: JobStorageTransaction
    {
        private readonly IAmazonDynamoDB _client;
        private readonly List<TransactWriteItem> _transactionItems;

        public DynamoDbWriteOnlyTransaction(IAmazonDynamoDB client)
        {
            _client = client;
            _transactionItems = new List<TransactWriteItem>();
        }
        
        public override void ExpireJob(string jobId, TimeSpan expireIn)
        {
            throw new NotImplementedException();
        }

        public override void PersistJob(string jobId)
        {
            throw new NotImplementedException();
        }

        public override void SetJobState(string jobId, IState state)
        {
            throw new NotImplementedException();
        }

        public override void AddJobState(string jobId, IState state)
        {
            throw new NotImplementedException();
        }

        public override void AddToQueue(string queue, string jobId)
        {
            throw new NotImplementedException();
        }

        public override void IncrementCounter(string key)
        {
            throw new NotImplementedException();
        }

        public override void IncrementCounter(string key, TimeSpan expireIn)
        {
            throw new NotImplementedException();
        }

        public override void DecrementCounter(string key)
        {
            throw new NotImplementedException();
        }

        public override void DecrementCounter(string key, TimeSpan expireIn)
        {
            throw new NotImplementedException();
        }

        public override void AddToSet(string key, string value)
        {
            AddToSet(key, value, 0);
        }

        public override void AddToSet(string key, string value, double score)
        {
            var update = new Update
            {
                TableName = "HangfireJobStorage",
                Key = new Dictionary<string, AttributeValue>
                {
                    { "Id", new AttributeValue { S = key } }
                },
                UpdateExpression = "ADD JobList :q",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":q", new AttributeValue { SS = new List<string> { value } } }
                }
            };
            
            _transactionItems.Add(new TransactWriteItem{ Update = update});
        }

        public override void RemoveFromSet(string key, string value)
        {
            var update = new Update
            {
                TableName = "HangfireJobStorage",
                Key = new Dictionary<string, AttributeValue>
                {
                    { "Id", new AttributeValue { S = key } }
                },
                UpdateExpression = "DELETE JobList :q",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {":q", new AttributeValue {SS = new List<string>{value}}}
                }
            };
            
            _transactionItems.Add(new TransactWriteItem{ Update = update});
        }

        public override void InsertToList(string key, string value)
        {
            var update = new Update
            {
                TableName = "HangfireJobStorage",
                Key = new Dictionary<string, AttributeValue>
                {
                    { "Id", new AttributeValue { S = key } }
                },
                UpdateExpression = "ADD JobList :q",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":q", new AttributeValue { SS = new List<string> { value } } }
                }
            };
            
            _transactionItems.Add(new TransactWriteItem{ Update = update});
        }

        public override void RemoveFromList(string key, string value)
        {
            var update = new Update
            {
                TableName = "HangfireJobStorage",
                Key = new Dictionary<string, AttributeValue>
                {
                    { "Id", new AttributeValue { S = key } }
                },
                UpdateExpression = "DELETE JobList :q",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {":q", new AttributeValue {SS = new List<string>{value}}}
                }
            };
            
            _transactionItems.Add(new TransactWriteItem{ Update = update});
        }

        public override void TrimList(string key, int keepStartingFrom, int keepEndingAt)
        {
            throw new NotImplementedException();
        }

        public override void SetRangeInHash(string key, IEnumerable<KeyValuePair<string, string>> keyValuePairs)
        {
            // _transaction.AddSaveItem(new DynamoRecord
            // {
            //     Id = key,
            //     Value = JsonConvert.SerializeObject(keyValuePairs)
            // });
        }

        public override void RemoveHash(string key)
        {
            throw new NotImplementedException();
        }

        public override void Commit()
        {
            var transactWriteItemsRequest = new TransactWriteItemsRequest()
            {
                TransactItems = _transactionItems,
                ReturnConsumedCapacity = ReturnConsumedCapacity.TOTAL
            };

            _client.TransactWriteItemsAsync(transactWriteItemsRequest).Wait();
        }
    }
}