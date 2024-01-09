using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime.Documents;
using Hangfire.Server;
using Newtonsoft.Json;
using Job = Hangfire.Common.Job;

namespace Hangfire.Storage.DynamoDb
{
    public class DynamoDbConnection : JobStorageConnection
    {
        private readonly IAmazonDynamoDB _client;

        public DynamoDbConnection(IAmazonDynamoDB client)
        {
            _client = client;
        }

        public override IWriteOnlyTransaction CreateWriteTransaction() => new DynamoDbWriteOnlyTransaction(_client);

        public override IDisposable AcquireDistributedLock(string resource, TimeSpan timeout)
        {
            return null;
        }

        public override string CreateExpiredJob(Job job, IDictionary<string, string> parameters, DateTime createdAt,
            TimeSpan expireIn)
        {
            throw new NotImplementedException();
        }

        public override IFetchedJob FetchNextJob(string[] queues, CancellationToken cancellationToken)
        {
            return new Entities.Job();
        }

        public override void SetJobParameter(string id, string name, string value)
        {
            throw new NotImplementedException();
        }

        public override string GetJobParameter(string id, string name)
        {
            throw new NotImplementedException();
        }

        public override JobData GetJobData(string jobId)
        {
            return null;
        }

        public override StateData GetStateData(string jobId)
        {
            throw new NotImplementedException();
        }

        public override void AnnounceServer(string serverId, ServerContext context)
        {
            if (serverId == null) throw new ArgumentNullException(nameof(serverId));
            if (context == null) throw new ArgumentNullException(nameof(context));

            _client.PutItemAsync(
                new PutItemRequest()
                {
                    TableName = "HangfireJobStorage",
                    Item = new Dictionary<string, AttributeValue>
                    {
                        {
                            "Id", new AttributeValue {S = $"server:{serverId}"}
                        },
                        {
                            "ServerId", new AttributeValue { S = serverId }
                        },
                        {
                            "Workers", new AttributeValue { N = context.WorkerCount.ToString() }
                        },
                        {
                            "Queues", new AttributeValue
                            {
                                L = context.Queues.Select(x => new AttributeValue { S = x }).ToList()
                            }
                        },
                        {
                            "CreatedOn", new AttributeValue { S = DateTime.UtcNow.ToString() }
                        },
                        {
                            "LastHeartbeat", new AttributeValue { S = DateTime.UtcNow.ToString() }
                        }
                    }
                }
            ).Wait();

            _client.UpdateItemAsync(
                new UpdateItemRequest
                {
                    TableName = "HangfireJobStorage",
                    Key = new Dictionary<string, AttributeValue>
                    {
                        { "Id", new AttributeValue { S = $"servers" } }
                    },
                    UpdateExpression = "ADD Servers :q",
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                    {
                        {":q", new AttributeValue {SS = new List<string>{serverId}}}
                    }
                }
            ).Wait();
        }

        public override void RemoveServer(string serverId)
        {
            if (serverId == null) throw new ArgumentNullException(nameof(serverId));
            
            _client.DeleteItemAsync(
                new DeleteItemRequest
                {
                    TableName = "HangfireJobStorage",
                    Key = new Dictionary<string, AttributeValue>
                    {
                        { "Id", new AttributeValue { S = $"server:{serverId}" } }
                    }
                }
            );
            
            _client.UpdateItemAsync(
                new UpdateItemRequest
                {
                    TableName = "HangfireJobStorage",
                    Key = new Dictionary<string, AttributeValue>
                    {
                        { "Id", new AttributeValue { S = $"servers" } }
                    },
                    UpdateExpression = "DELETE Servers :q",
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                    {
                        {":q", new AttributeValue {SS = new List<string>{serverId}}}
                    }
                }
            ).Wait();
        }

        public override void Heartbeat(string serverId)
        {
            if (serverId == null) throw new ArgumentNullException(nameof(serverId));
            
            _client.UpdateItemAsync(
                new UpdateItemRequest
                {
                    TableName = "HangfireJobStorage",
                    Key = new Dictionary<string, AttributeValue>
                    {
                        { "Id", new AttributeValue { S = $"server:{serverId}" } }
                    },
                    AttributeUpdates = new Dictionary<string, AttributeValueUpdate>
                    {
                        {
                            "LastHeartbeat", new AttributeValueUpdate
                            {
                                Action = AttributeAction.PUT,
                                Value = { S = DateTime.UtcNow.ToString() }
                            }
                        }
                    }
                }
            ).Wait();
        }

        public override int RemoveTimedOutServers(TimeSpan timeOut)
        {
            var removedServers = 0;

            // var servers = _dynamoDb.Fetch<Servers>("servers").Result;
            //
            // foreach (var id in servers.ServerIds)
            // {
            //     var server = _dynamoDb.Fetch<Entities.Server>($"server:{id}").Result;
            //
            //     if (server.LastHeartbeat < DateTime.UtcNow.Subtract(timeOut))
            //     {
            //         RemoveServer(id);
            //         removedServers++;
            //     }
            // }

            return removedServers;
        }

        public override HashSet<string> GetAllItemsFromSet(string key)
        {
            throw new NotImplementedException();
        }

        public override string GetFirstByLowestScoreFromSet(string key, double fromScore, double toScore)
        {
            var request = new GetItemRequest
            {
                TableName = "HangfireJobStorage",
                Key = new Dictionary<string, AttributeValue>
                {
                    { "Id", new AttributeValue { S = key } }
                }
            };
            
            var response = _client.GetItemAsync(request).Result;

            return String.Empty;
        }

        public override void SetRangeInHash(string key, IEnumerable<KeyValuePair<string, string>> keyValuePairs)
        {
            throw new NotImplementedException();
        }

        public override Dictionary<string, string> GetAllEntriesFromHash(string key)
        {
            var request = new GetItemRequest
            {
                TableName = "HangfireJobStorage",
                Key = new Dictionary<string, AttributeValue>
                {
                    { "Id", new AttributeValue { S = key } }
                }
            };
            
            var response = _client.GetItemAsync(request).Result;

            
            if (response != null && response.Item.Count() != 0)
            {
                return response.Item.ToDictionary(x => x.Key, x => x.Value.ToString());
            }

            return null;
        }
    }
}