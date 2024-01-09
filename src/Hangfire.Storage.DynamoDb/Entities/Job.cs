namespace Hangfire.Storage.DynamoDb.Entities
{
    public class Job: IFetchedJob
    {
        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public void RemoveFromQueue()
        {
            throw new System.NotImplementedException();
        }

        public void Requeue()
        {
            throw new System.NotImplementedException();
        }

        public string JobId { get; }
    }
}