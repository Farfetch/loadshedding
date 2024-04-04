using System;
using Farfetch.LoadShedding.Tasks;

namespace Farfetch.LoadShedding.Events.Args
{
    /// <summary>
    /// Event args for the task dequeued event.
    /// </summary>
    public class ItemDequeuedEventArgs : TaskQueueEventArgs
    {
        internal ItemDequeuedEventArgs(Priority priority, TimeSpan queueTime, IReadOnlyCounter queueCounter)
            : base(priority, queueCounter)
        {
            this.QueueTime = queueTime;
        }

        /// <summary>
        /// Gets the time waiting in the queue.
        /// </summary>
        public TimeSpan QueueTime { get; }
    }
}
