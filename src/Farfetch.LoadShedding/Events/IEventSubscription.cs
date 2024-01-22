namespace Farfetch.LoadShedding.Events
{
    /// <summary>
    /// Represents a reference to the event subscription.
    /// </summary>
    public interface IEventSubscription
    {
        /// <summary>
        /// Cancels the event subscription.
        /// </summary>
        void Cancel();
    }
}
