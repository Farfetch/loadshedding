using System;
using Farfetch.LoadShedding.Tasks;

namespace Farfetch.LoadShedding.Events.Args
{
    /// <summary>
    /// Event args for task processed event.
    /// </summary>
    public class ItemProcessedEventArgs : TaskItemEventArgs
    {
        internal ItemProcessedEventArgs(ITaskItem taskItem, IReadOnlyCounter concurrencyCounter)
            : base(taskItem, concurrencyCounter)
        {
        }

        /// <summary>
        /// Gets time spent to process the task.
        /// </summary>
        public TimeSpan ProcessingTime => TaskItem.ProcessingTime;
    }
}
