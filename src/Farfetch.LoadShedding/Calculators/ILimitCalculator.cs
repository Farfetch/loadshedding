using Farfetch.LoadShedding.Configurations;

namespace Farfetch.LoadShedding.Calculators
{
    /// <summary>
    /// Represents an interface that is responsible for the abstraction of different types of limit calculators.
    /// </summary>
    public interface ILimitCalculator
    {
        /// <summary>
        /// Responsible for calculate and return a limit value.
        /// </summary>
        /// <param name="context">A concurrency context properties.</param>
        /// <returns>An integer limit based on the calculator logic and the current concurrency context.</returns>
        int CalculateLimit(IConcurrencyContext context);
    }
}
