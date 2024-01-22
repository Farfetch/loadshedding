namespace Farfetch.LoadShedding.Exceptions
{
    /// <summary>
    ///  Represents an exception where the queue limit defined was reached.
    /// </summary>
    public class QueueLimitReachedException : LimitReachedException
    {
        /// <summary>
        /// Constructs the custom exception.
        /// </summary>
        /// <param name="limit">The current queue limit.</param>
        public QueueLimitReachedException(int limit)
            : base($"The maximum queue limit of {limit} is reached.")
        {

        }
    }
}
