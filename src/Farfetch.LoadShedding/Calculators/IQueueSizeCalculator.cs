using Farfetch.LoadShedding.Configurations;

namespace Farfetch.LoadShedding.Calculators
{
    /// <summary>
    /// Represents an interface that is responsible for the abstraction of different types of queue size calculators.
    /// </summary>
    public interface IQueueSizeCalculator
    {
        /// <summary>
        /// Responsible for calculate and return a queue size value.
        /// </summary>
        /// <param name="context">A concurrency context properties.</param>
        /// <returns>An integer value based on the calculator logic and the current concurrency context.</returns>
        int CalculateQueueSize(IConcurrencyContext context);
    }
}
