using Farfetch.LoadShedding.Tasks;

namespace Farfetch.LoadShedding.Events.Args
{
    /// <summary>
    /// Event args for the task enqueued event.
    /// </summary>
    public class ItemEnqueuedEventArgs : ItemEventArgs
    {
        internal ItemEnqueuedEventArgs(Priority priority, int queueLimit, int queueCount)
            : base(priority)
        {
            this.QueueLimit = queueLimit;
            this.QueueCount = queueCount;
        }

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
