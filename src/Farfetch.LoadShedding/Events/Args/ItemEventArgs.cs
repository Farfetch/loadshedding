using System.Collections.Generic;
using Farfetch.LoadShedding.Tasks;

namespace Farfetch.LoadShedding.Events.Args
{
    /// <summary>
    /// Event args for a task item event.
    /// </summary>
    public abstract class ItemEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemEventArgs"/> class.
        /// </summary>
        /// <param name="taskItem">The task item assigned to the request</param>
        protected ItemEventArgs(ITaskItem taskItem)
        {
            TaskItem = taskItem;
        }

        /// <summary>
        /// Gets the priority of the task.
        /// </summary>
        public Priority Priority => TaskItem.Priority;

        /// <summary>
        /// Gets the labels of the task.
        /// </summary>
        public IDictionary<string, string> Labels => TaskItem.Labels;

        /// <summary>
        /// Gets the <see cref="ITaskItem"/> of the event args.
        /// </summary>
        protected ITaskItem TaskItem { get; }
    }
}
