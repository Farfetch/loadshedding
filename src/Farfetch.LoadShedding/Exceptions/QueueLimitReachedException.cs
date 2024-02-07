namespace Farfetch.LoadShedding.Exceptions
{
    /// <summary>
    ///  Represents an exception where the queue limit defined was reached.
    /// </summary>
    public class QueueLimitReachedException : LimitReachedException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueueLimitReachedException"/> class.
        /// Constructs the custom exception.
        /// </summary>
        /// <param name="limit">The current queue limit.</param>
        public QueueLimitReachedException(int limit)
            : base($"The maximum queue limit of {limit} is reached.")
        {
        }
    }
}
