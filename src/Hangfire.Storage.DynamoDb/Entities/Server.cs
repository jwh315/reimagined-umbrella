using System;
using Amazon.DynamoDBv2.DataModel;

namespace Hangfire.Storage.DynamoDb.Entities
{
    public class Server
    {
        public string ServerId { get; set; }

        public int Workers { get; set; }

        public string[] Queues { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime LastHeartbeat { get; set; }
    }
}