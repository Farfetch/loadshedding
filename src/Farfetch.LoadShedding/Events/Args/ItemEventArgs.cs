using Farfetch.LoadShedding.Tasks;

namespace Farfetch.LoadShedding.Events.Args
{
    /// <summary>
    /// Event args for task rejected event.
    /// </summary>
    public class ItemEventArgs
    {
        protected ItemEventArgs(Priority priority)
        {
            this.Priority = priority;
        }

        /// <summary>
        /// The priority of the task.
        /// </summary>
        public Priority Priority { get; }
    }
}
