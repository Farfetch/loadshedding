using System;
using Farfetch.LoadShedding.Tasks;

namespace Farfetch.LoadShedding.Events.Args
{
    /// <summary>
    /// Event args for task processed event.
    /// </summary>
    public class ItemProcessedEventArgs : ItemEventArgs
    {
        internal ItemProcessedEventArgs(Priority priority, TimeSpan processingTime, TimeSpan waitingTime, int concurrencyLimit, int concurrencyCount, int queueCount)
            : base(priority)
        {
            this.ProcessingTime = processingTime;
            this.WaitingTime = waitingTime;
            this.ConcurrencyLimit = concurrencyLimit;
            this.ConcurrencyCount = concurrencyCount;
            this.QueueCount = queueCount;
        }

        /// <summary>
        /// Gets time spent to process the task.
        /// </summary>
        public TimeSpan ProcessingTime { get; }

        /// <summary>
        /// Gets time waiting in the queue.
        /// </summary>
        public TimeSpan WaitingTime { get; }

        /// <summary>
        /// Gets the current concurrency limit.
        /// </summary>
        public int ConcurrencyLimit { get; }

        /// <summary>
        /// Gets the current concurrency items count.
        /// </summary>
        public int ConcurrencyCount { get; }

        /// <summary>
        /// Gets the current number of items in the queue.
        /// </summary>
        public int QueueCount { get; }
    }
}
