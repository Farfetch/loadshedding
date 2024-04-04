using Farfetch.LoadShedding.Tasks;

namespace Farfetch.LoadShedding.Events.Args
{
    /// <summary>
    /// Event args for the task enqueued event.
    /// </summary>
    public class ItemEnqueuedEventArgs : TaskQueueEventArgs
    {
        internal ItemEnqueuedEventArgs(Priority priority, IReadOnlyCounter queueCounter)
            : base(priority, queueCounter)
        {
        }
    }
}
