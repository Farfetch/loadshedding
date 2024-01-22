using Farfetch.LoadShedding.Tasks;

namespace Farfetch.LoadShedding.Events.Args
{
    /// <summary>
    /// Event args for task rejected event.
    /// </summary>
    public class ItemRejectedEventArgs : ItemEventArgs
    {
        internal ItemRejectedEventArgs(Priority priority, string reason)
            : base(priority)
        {
            Reason = reason;
        }

        public string Reason { get; }
    }
}
