namespace Farfetch.LoadShedding.Configurations
{
    /// <summary>
    /// Represents the concurrency context contract.
    /// </summary>
    public interface IConcurrencyContext
    {
        /// <summary>
        /// Gets the maximum concurrency limit.
        /// </summary>
        int MaxConcurrencyLimit { get; }

        /// <summary>
        /// Gets the maximum queue size.
        /// </summary>
        int MaxQueueSize { get; }

        /// <summary>
        /// Gets the current queue count.
        /// </summary>
        int CurrentQueueCount { get; }

        /// <summary>
        /// Gets the current average round trip time.
        /// </summary>
        long AvgRTT { get; }

        /// <summary>
        /// Gets the min round trip time.
        /// </summary>
        long MinRTT { get; }

        /// <summary>
        /// Gets the previous average round trip time.
        /// </summary>
        long PreviousAvgRTT { get; }
    }
}
