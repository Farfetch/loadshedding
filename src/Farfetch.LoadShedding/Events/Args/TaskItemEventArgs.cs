using Farfetch.LoadShedding.Tasks;

namespace Farfetch.LoadShedding.Events.Args
{
    /// <summary>
    /// Event args for task item event.
    /// </summary>
    public class TaskItemEventArgs : ItemEventArgs
    {
        private readonly IReadOnlyCounter _concurrencyCounter;

        internal TaskItemEventArgs(Priority priority, IReadOnlyCounter concurrencyCounter)
            : base(priority)
        {
            _concurrencyCounter = concurrencyCounter;
        }

        /// <summary>
        /// Gets the current concurrency limit.
        /// </summary>
        public int ConcurrencyLimit => _concurrencyCounter.Limit;

        /// <summary>
        /// Gets the current concurrency items count.
        /// </summary>
        public int ConcurrencyCount => _concurrencyCounter.Count;
    }
}
