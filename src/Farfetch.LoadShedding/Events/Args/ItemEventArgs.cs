using Farfetch.LoadShedding.Tasks;

namespace Farfetch.LoadShedding.Events.Args
{
    /// <summary>
    /// Event args for task rejected event.
    /// </summary>
    public class ItemEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemEventArgs"/> class.
        /// </summary>
        /// <param name="priority">The priority assigned to the request</param>
        protected ItemEventArgs(Priority priority)
        {
            this.Priority = priority;
        }

        /// <summary>
        /// Gets the priority of the task.
        /// </summary>
        public Priority Priority { get; }
    }
}
