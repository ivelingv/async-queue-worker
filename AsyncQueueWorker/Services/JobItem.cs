using System;

namespace AsyncQueueWorker.Services
{
    public enum Status
    {
        Pending,
        InProgress,
        Complated,
    }

    public class JobItem
    {
        public string JobId { get; }
        public long Timestamp { get; }
        public Status Status { get; private set; }

        public JobItem(string jobId)
        {
            JobId = jobId;
            Status = Status.Pending;
            Timestamp = DateTime.Now.Ticks;
        }

        public void SetStatus(Status status)
        {
            if (Status != Status.Pending
                && status == Status.InProgress)
            {
                throw new InvalidOperationException(
                    "The job is already in progress");
            }

            if (Status != Status.InProgress
                && status == Status.Complated)
            {
                throw new InvalidOperationException(
                    "The job is not in progress");
            }

            if (Status == Status.Complated)
            {
                throw new InvalidOperationException(
                    "The job is completed");
            }

            Status = status;
        }
    }
}
