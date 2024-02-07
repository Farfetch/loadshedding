using System;
using System.Threading.Tasks;
using Farfetch.LoadShedding.Tasks;
using Microsoft.AspNetCore.Http;

namespace Farfetch.LoadShedding.AspNetCore.Resolvers
{
    internal class CustomPriorityResolver : IPriorityResolver
    {
        private readonly Func<HttpContext, Task<Priority>> _resolverFunc;

        public CustomPriorityResolver(Func<HttpContext, Task<Priority>> resolverFunc)
        {
            this._resolverFunc = resolverFunc;
        }

        public Task<Priority> ResolveAsync(HttpContext context)
        {
            return this._resolverFunc.Invoke(context);
        }
    }
}
