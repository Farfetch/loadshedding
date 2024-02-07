using System;
using Farfetch.LoadShedding.Tasks;

namespace Farfetch.LoadShedding.Events.Args
{
    /// <summary>
    /// Event args for the task dequeued event.
    /// </summary>
    public class ItemDequeuedEventArgs : ItemEventArgs
    {
        internal ItemDequeuedEventArgs(Priority priority, TimeSpan queueTime, int queueLimit, int queueCount)
            : base(priority)
        {
            this.QueueTime = queueTime;
            this.QueueLimit = queueLimit;
            this.QueueCount = queueCount;
        }

        /// <summary>
        /// Gets the time waiting in the queue.
        /// </summary>
        public TimeSpan QueueTime { get; }

        /// <summary>
        /// Gets the maximum number of items in the queue.
        /// </summary>
        public int QueueLimit { get; }

        /// <summary>
        /// Gets the current number of items in the queue.
        /// </summary>
        public int QueueCount { get; }
    }
}
