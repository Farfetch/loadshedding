namespace Farfetch.LoadShedding.Events.Args
{
    /// <summary>
    /// Events args for limits changed event.
    /// </summary>
    public class LimitChangedEventArgs
    {
        internal LimitChangedEventArgs(int limit)
        {
            this.Limit = limit;
        }

        /// <summary>
        /// The current limit.
        /// </summary>
        public int Limit { get; }
    }
}
