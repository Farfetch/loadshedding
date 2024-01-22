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
        /// The time waiting in the queue.
        /// </summary>
        public TimeSpan QueueTime { get; }

        public int QueueLimit { get; }

        public int QueueCount { get; }
    }
}
