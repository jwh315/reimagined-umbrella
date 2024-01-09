using System.Collections.Generic;

namespace Hangfire.Storage.DynamoDb.Entities
{
    public class Servers
    {
        public HashSet<string> ServerIds { get; set; } = new HashSet<string>();
    }
}