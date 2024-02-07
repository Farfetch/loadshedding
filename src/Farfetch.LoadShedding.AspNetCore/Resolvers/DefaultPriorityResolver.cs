using System.Threading.Tasks;
using Farfetch.LoadShedding.Tasks;
using Microsoft.AspNetCore.Http;

namespace Farfetch.LoadShedding.AspNetCore.Resolvers
{
    /// <summary>
    /// This resolver always returns the default priority (Normal).
    /// </summary>
    internal class DefaultPriorityResolver : IPriorityResolver
    {
        private static readonly Task<Priority> s_defaultResult = Task.FromResult(Priority.Normal);

        public Task<Priority> ResolveAsync(HttpContext context)
        {
            return s_defaultResult;
        }
    }
}
