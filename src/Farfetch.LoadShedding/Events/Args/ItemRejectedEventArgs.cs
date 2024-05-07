using Farfetch.LoadShedding.Tasks;

namespace Farfetch.LoadShedding.Events.Args
{
    /// <summary>
    /// Event args for task rejected event.
    /// </summary>
    public class ItemRejectedEventArgs : ItemEventArgs
    {
        internal ItemRejectedEventArgs(ITaskItem taskItem, string reason)
            : base(taskItem)
        {
            Reason = reason;
        }

        /// <summary>
        /// Gets the reason for the item being rejected.
        /// </summary>
        public string Reason { get; }
    }
}
