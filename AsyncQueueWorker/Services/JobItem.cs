using System;

namespace AsyncQueueWorker.Services
{
    public class JobItem
    {
        public string JobId { get; }
        public long Timestamp { get; }

        public JobItem(string jobId)
        {
            JobId = jobId;
            Timestamp = DateTime.Now.Ticks;
        }
    }
}
