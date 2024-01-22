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

        public int QueueLimit { get; }

        public int QueueCount { get; }
    }
}
