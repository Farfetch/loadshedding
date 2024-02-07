using System;
using Farfetch.LoadShedding.Tasks;

namespace Farfetch.LoadShedding.Events.Args
{
    /// <summary>
    /// Event args for task processed event.
    /// </summary>
    public class ItemProcessedEventArgs : ItemEventArgs
    {
        internal ItemProcessedEventArgs(Priority priority, TimeSpan processingTime, int concurrencyLimit, int concurrencyCount)
            : base(priority)
        {
            this.ProcessingTime = processingTime;
            this.ConcurrencyLimit = concurrencyLimit;
            this.ConcurrencyCount = concurrencyCount;
        }

        /// <summary>
        /// Gets time spent to process the task.
        /// </summary>
        public TimeSpan ProcessingTime { get; }

        /// <summary>
        /// Gets the current concurrency limit.
        /// </summary>
        public int ConcurrencyLimit { get; }

        /// <summary>
        /// Gets the current concurrency items count.
        /// </summary>
        public int ConcurrencyCount { get; }
    }
}
