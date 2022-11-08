using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncQueueWorker.Services
{
    public class BackgroundWorkerService
    {
        protected static object Locker = new();

        protected Queue<JobItem> PendingItems { get; } = new();
        protected CancellationTokenSource CancellationTokenSource { get; } = new();
        protected List<Thread> Threads { get; } = new();

        private void DoWork(object parameter)
        {
            var cancellationToken = (CancellationToken)parameter;

            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine(
                        $"Stopping thread [{Thread.CurrentThread.ManagedThreadId}]");
                    return;
                }

                Thread.Sleep(5000);

                JobItem jobItem = null;

                lock (Locker)
                {
                    PendingItems.TryDequeue(out jobItem);
                }

                if (jobItem is null)
                {
                    Console.WriteLine(
                        $"There are not jobs " +
                        $"for processing in thread " +
                        $"[{Thread.CurrentThread.ManagedThreadId}]");

                    continue;
                }    

                for (var i = 0; i < 10; i++)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Console.WriteLine(
                            $"Stopping thread [{Thread.CurrentThread.ManagedThreadId}]");
                        return;
                    }

                    Console.WriteLine($"Doing work for job item " +
                        $"[{jobItem.JobId}] [{i + 1}/{10}] " +
                        $"[{Thread.CurrentThread.ManagedThreadId}]");

                    Thread.Sleep(1000);
                }
            }
        }

        public void Start()
        {
            var thread = new Thread(DoWork);
            thread.Start(CancellationTokenSource.Token);
            Threads.Add(thread);
        }

        public void Queue(string jobId)
        {
            lock(Locker)
            {
                var hasItem = PendingItems
                    .ToList()
                    .Where(e => e.JobId == jobId)
                    .Any();

                if (!hasItem)
                {
                    PendingItems.Enqueue(
                        new JobItem(jobId));
                }
            }
        }

        public JobItem[] Pending()
        {
            lock (Locker)
            {
                return PendingItems.ToArray();
            }
        }

        public void Stop()
        {
            CancellationTokenSource.Cancel();
        }
    }
}
