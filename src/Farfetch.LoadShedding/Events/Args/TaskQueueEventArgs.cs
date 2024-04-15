using Farfetch.LoadShedding.Tasks;

namespace Farfetch.LoadShedding.Events.Args
{
    /// <summary>
    /// Event args for the task queue event.
    /// </summary>
    public class TaskQueueEventArgs : ItemEventArgs
    {
        private readonly IReadOnlyCounter _queueCounter;

        internal TaskQueueEventArgs(Priority priority, IReadOnlyCounter queueCounter)
            : base(priority)
        {
            this._queueCounter = queueCounter;
        }

        /// <summary>
        /// Gets the maximum number of items in the queue.
        /// </summary>
        public int QueueLimit => _queueCounter.Limit;

        /// <summary>
        /// Gets the current number of items in the queue.
        /// </summary>
        public int QueueCount => _queueCounter.Count;
    }
}
