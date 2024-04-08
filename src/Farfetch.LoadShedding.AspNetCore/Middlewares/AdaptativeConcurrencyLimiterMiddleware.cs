using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Farfetch.LoadShedding.AspNetCore.Resolvers;
using Farfetch.LoadShedding.Exceptions;
using Farfetch.LoadShedding.Limiters;
using Microsoft.AspNetCore.Http;

[assembly: InternalsVisibleTo("Farfetch.LoadShedding.AspNetCore.Tests")]
[assembly: InternalsVisibleTo("Farfetch.LoadShedding.BenchmarkTests")]

namespace Farfetch.LoadShedding.AspNetCore.Middlewares
{
    internal class AdaptativeConcurrencyLimiterMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAdaptativeConcurrencyLimiter _limiter;
        private readonly IPriorityResolver _priorityResolver;

        public AdaptativeConcurrencyLimiterMiddleware(
            RequestDelegate next,
            IAdaptativeConcurrencyLimiter limiter,
            IPriorityResolver priorityResolver)
        {
            this._next = next;
            this._limiter = limiter;
            this._priorityResolver = priorityResolver ?? new DefaultPriorityResolver();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                var priority = await this._priorityResolver
                    .ResolveAsync(context)
                    .ConfigureAwait(false);

                await this._limiter
                    .ExecuteAsync(priority, () => this._next.Invoke(context), context.Request.Method.ToUpperInvariant(), context.RequestAborted)
                    .ConfigureAwait(false);
            }
            catch (LimitReachedException)
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            }
            catch (QueueTimeoutException)
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            }
        }
    }
}
