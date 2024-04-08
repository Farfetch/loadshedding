using System;
using Farfetch.LoadShedding.Tasks;

namespace Farfetch.LoadShedding.Events.Args
{
    /// <summary>
    /// Event args for the task dequeued event.
    /// </summary>
    public class ItemDequeuedEventArgs : TaskQueueEventArgs
    {
        internal ItemDequeuedEventArgs(ITaskItem taskItem, IReadOnlyCounter queueCounter)
            : base(taskItem, queueCounter)
        {
        }

        /// <summary>
        /// Gets the time waiting in the queue.
        /// </summary>
        public TimeSpan QueueTime => TaskItem.WaitingTime;
    }
}
