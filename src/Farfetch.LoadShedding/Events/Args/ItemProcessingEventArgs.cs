using Farfetch.LoadShedding.Tasks;

namespace Farfetch.LoadShedding.Events.Args
{
    /// <summary>
    /// Event args for task procssing event.
    /// </summary>
    public class ItemProcessingEventArgs : ItemEventArgs
    {
        internal ItemProcessingEventArgs(Priority priority, int concurrencyLimit, int concurrencyCount) : base(priority)
        {
            this.ConcurrencyLimit = concurrencyLimit;
            this.ConcurrencyCount = concurrencyCount;
        }

        /// <summary>
        /// The current concurrency limit.
        /// </summary>
        public int ConcurrencyLimit { get; }

        /// <summary>
        /// The current concurrency items count.
        /// </summary>
        public int ConcurrencyCount { get; }
    }
}
