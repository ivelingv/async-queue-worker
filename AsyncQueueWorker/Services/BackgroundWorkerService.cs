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

        protected List<JobItem> PendingItems { get; } = new();
        protected CancellationTokenSource CancellationTokenSource { get; private set; } = new();
        protected List<Thread> Threads { get; } = new();
        protected ManualResetEvent Event { get; } = new(true);

        private void DoWork(object parameter)
        {
            var cancellationToken = (CancellationToken)parameter;

            while (true)
            {
                var isResuming = false;
                if (cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine(
                        $"Stopping thread [{Thread.CurrentThread.ManagedThreadId}]");
                    return;
                }

                Thread.Sleep(3000);

                while(!Event.WaitOne(1000))
                {
                    isResuming = true;
                    Console.WriteLine(
                       $"Paused thread [{Thread.CurrentThread.ManagedThreadId}]");
                }

                if (isResuming)
                {
                    Console.WriteLine(
                      $"Resuming thread [{Thread.CurrentThread.ManagedThreadId}]");
                }

                JobItem jobItem = null;

                lock (Locker)
                {
                    jobItem = PendingItems
                        .Where(e => e.Status == Status.Pending)
                        .OrderBy(e => e.Timestamp)
                        .FirstOrDefault();

                    if (jobItem is null)
                    {
                        Console.WriteLine(
                            $"There are not jobs " +
                            $"for processing in thread " +
                            $"[{Thread.CurrentThread.ManagedThreadId}]");

                        continue;
                    }

                    jobItem.SetStatus(Status.InProgress);
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

                jobItem.SetStatus(Status.Complated);
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
                    .Where(e => e.JobId == jobId)
                    .Any();

                if (!hasItem)
                {
                    PendingItems.Add(
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

        public JobItem GetJobStatus(string jobId)
        {
            return PendingItems
                .Where(e => e.JobId == jobId)
                .FirstOrDefault();
        }

        public void Stop()
        {
            CancellationTokenSource.Cancel();
            CancellationTokenSource = new CancellationTokenSource();
        }

        public void Pause()
        {
            Event.Reset();
        }

        public void Resume()
        {
            Event.Set();
        }
    }
}
