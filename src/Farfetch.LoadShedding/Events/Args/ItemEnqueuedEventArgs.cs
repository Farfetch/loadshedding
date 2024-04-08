using Farfetch.LoadShedding.Tasks;

namespace Farfetch.LoadShedding.Events.Args
{
    /// <summary>
    /// Event args for the task enqueued event.
    /// </summary>
    public class ItemEnqueuedEventArgs : TaskQueueEventArgs
    {
        internal ItemEnqueuedEventArgs(ITaskItem taskItem, IReadOnlyCounter queueCounter)
            : base(taskItem, queueCounter)
        {
        }
    }
}
