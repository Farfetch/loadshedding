using Farfetch.LoadShedding.Tasks;

namespace Farfetch.LoadShedding.Events.Args
{
    /// <summary>
    /// Event args for task processing event.
    /// </summary>
    public class ItemProcessingEventArgs : TaskItemEventArgs
    {
        internal ItemProcessingEventArgs(Priority priority, IReadOnlyCounter concurrencyCounter)
            : base(priority, concurrencyCounter)
        {
        }
    }
}
