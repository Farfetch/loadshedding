using Farfetch.LoadShedding.Tasks;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Farfetch.LoadShedding.AspNetCore.Resolvers
{
    /// <summary>
    /// Priority resolver contract.
    /// </summary>
    public interface IPriorityResolver
    {
        /// <summary>
        /// Resolves the task priority.
        /// </summary>
        /// <param name="context">The HttpContext instance.</param>
        /// <returns>The task priority.</returns>
        Task<Priority> ResolveAsync(HttpContext context);
    }
}
