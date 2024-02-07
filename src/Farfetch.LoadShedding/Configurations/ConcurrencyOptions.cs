using System.Threading;
using Farfetch.LoadShedding.Exceptions;

namespace Farfetch.LoadShedding.Configurations
{
    /// <summary>
    /// Settings to configure the Adaptative Limiter.
    /// </summary>
    public class ConcurrencyOptions
    {
        /// <summary>
        /// Gets or sets the minimum concurrency limit.
        /// </summary>
        public int MinConcurrencyLimit { get; set; } = 5;

        /// <summary>
        /// Gets or sets the initial concurrency limit.
        /// </summary>
        public int InitialConcurrencyLimit { get; set; } = 5;

        /// <summary>
        /// Gets or sets the maximum concurrency limit.
        /// </summary>
        public int MaxConcurrencyLimit { get; set; } = 500;

        /// <summary>
        /// Gets or sets the tolerance.
        /// </summary>
        public double Tolerance { get; set; } = 1.5;

        /// <summary>
        /// Gets or sets the minimum queue size.
        /// </summary>
        public int MinQueueSize { get; set; } = 20;

        /// <summary>
        /// Gets or sets the initial queue size.
        /// </summary>
        public int InitialQueueSize { get; set; } = 20;

        /// <summary>
        /// Gets or sets the queue waiting timeout, when the timeout is reached the task will be canceled and will throw an OperationCanceledException.
        /// </summary>
        public int QueueTimeoutInMs { get; set; } = Timeout.Infinite;

        internal void Validate()
        {
            const int CommonMinThreshold = 0, MinToleranceThreshold = 1;

            if (this.IsLimitQueuePropertiesWithInvalidValues(CommonMinThreshold))
            {
                throw new InvalidConfigurationException($"The value of {nameof(this.MinConcurrencyLimit)}, {nameof(this.InitialConcurrencyLimit)}, {nameof(this.MaxConcurrencyLimit)}," +
                    $" {nameof(this.MinQueueSize)}, or {nameof(this.InitialQueueSize)} should be greater than {CommonMinThreshold}");
            }

            if (this.Tolerance <= MinToleranceThreshold)
            {
                throw new InvalidConfigurationException($"The value of {nameof(this.Tolerance)} should be greater than {MinToleranceThreshold}");
            }

            if (this.MinConcurrencyLimit >= this.MaxConcurrencyLimit)
            {
                throw new InvalidConfigurationException($"The value of {nameof(this.MaxConcurrencyLimit)} should be greater than the {nameof(this.MinConcurrencyLimit)}");
            }

            if (this.MinConcurrencyLimit > this.InitialConcurrencyLimit || this.MaxConcurrencyLimit < this.InitialConcurrencyLimit)
            {
                throw new InvalidConfigurationException($"The value of {nameof(this.InitialConcurrencyLimit)} should be greater than {nameof(this.MinConcurrencyLimit)} " +
                    $"and less than {nameof(this.MaxConcurrencyLimit)}");
            }

            if (this.MinQueueSize > this.InitialQueueSize)
            {
                throw new InvalidConfigurationException($"The value of {nameof(this.InitialQueueSize)} should be greater than the {nameof(this.MinQueueSize)}");
            }
        }

        private bool IsLimitQueuePropertiesWithInvalidValues(int threshold) =>
            this.MinConcurrencyLimit <= threshold
            || this.InitialConcurrencyLimit <= threshold
            || this.MaxConcurrencyLimit <= threshold
            || this.MinQueueSize <= threshold
            || this.InitialQueueSize <= threshold;
    }
}
