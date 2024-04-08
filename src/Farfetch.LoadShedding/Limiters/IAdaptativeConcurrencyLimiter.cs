using System;
using System.Threading;
using System.Threading.Tasks;
using Farfetch.LoadShedding.Tasks;

namespace Farfetch.LoadShedding.Limiters
{
    /// <summary>
    /// Defines the contract that the limiters should follow.
    /// </summary>
    public interface IAdaptativeConcurrencyLimiter
    {
        /// <summary>
        /// Responsible for managing the current request.
        /// </summary>
        /// <param name="priority">The <see cref="Priority"/> of the execution of the task.</param>
        /// <param name="function">A function that represents the request that has been processed.</param>
        /// <param name="method">The method of the execution of the task.</param>
        /// <param name="cancellationToken">A cancellation token is used to signal that the running operation should be stopped.</param>
        /// <returns></returns>
        Task ExecuteAsync(Priority priority, Func<Task> function, string method = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Responsible for managing the current request.
        /// </summary>
        /// <param name="function">A function that represents the request that has been processed.</param>
        /// <param name="method">The method of the execution of the task.</param>
        /// <param name="cancellationToken">A cancellation token is used to signal that the running operation should be stopped.</param>
        /// <returns></returns>
        Task ExecuteAsync(Func<Task> function, string method = null, CancellationToken cancellationToken = default);
    }
}
