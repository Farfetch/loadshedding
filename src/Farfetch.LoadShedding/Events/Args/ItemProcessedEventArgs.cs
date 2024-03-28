using System;
using Farfetch.LoadShedding.Tasks;

namespace Farfetch.LoadShedding.Events.Args
{
    /// <summary>
    /// Event args for task processed event.
    /// </summary>
    public class ItemProcessedEventArgs : TaskItemEventArgs
    {
        internal ItemProcessedEventArgs(Priority priority, TimeSpan processingTime, IReadOnlyCounter concurrencyCounter)
            : base(priority, concurrencyCounter)
        {
            this.ProcessingTime = processingTime;
        }

        /// <summary>
        /// Gets time spent to process the task.
        /// </summary>
        public TimeSpan ProcessingTime { get; }
    }
}
