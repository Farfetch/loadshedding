using System;
using System.Threading.Tasks;
using Farfetch.LoadShedding.AspNetCore.Resolvers;
using Farfetch.LoadShedding.Configurations;
using Farfetch.LoadShedding.Tasks;
using Microsoft.AspNetCore.Http;

namespace Farfetch.LoadShedding.AspNetCore.Options
{
    /// <summary>
    /// Options for adaptative concurrency limiter.
    /// </summary>
    public class AdaptativeLimiterOptions
    {
        internal AdaptativeLimiterOptions()
        {
            this.ConcurrencyOptions = new ConcurrencyOptions();
        }

        /// <summary>
        /// Gets the Concurrency Options.
        /// </summary>
        public ConcurrencyOptions ConcurrencyOptions { get; }

        /// <summary>
        /// Gets or sets and sets the PriorityResolver.
        /// </summary>
        internal IPriorityResolver PriorityResolver { get; set; }

        /// <summary>
        /// Sets the EndpointPriorityResolver, it loads the priority from the controller action attribute EndpointPriorityAttribute.
        /// </summary>
        /// <returns>The AdaptativeLimiterOptions.</returns>
        public AdaptativeLimiterOptions UseEndpointPriorityResolver()
            => this.UsePriorityResolver(new EndpointPriorityResolver());

        /// <summary>
        /// Sets the HttpHeaderPriorityResolver, it converts the header X-Priority to the request priority (critical, normal, noncritical).
        /// </summary>
        /// <returns>AdaptativeLimiterOptions</returns>
        public AdaptativeLimiterOptions UseHeaderPriorityResolver()
            => this.UseHeaderPriorityResolver(HttpHeaderPriorityResolver.DefaultPriorityHeaderName);

        /// <summary>
        /// Sets the HttpHeaderPriorityResolver, it converts the header {{headerName}} to the request priority (critical, normal, noncritical).
        /// </summary>
        /// <param name="headerName">The name of the header with the priority value.</param>
        /// <returns>AdaptativeLimiterOptions</returns>
        public AdaptativeLimiterOptions UseHeaderPriorityResolver(string headerName)
            => this.UsePriorityResolver(new HttpHeaderPriorityResolver(headerName));

        /// <summary>
        /// Sets a custom priority resolver function.
        /// </summary>
        /// <param name="priorityResolverFunc">Delegate to resolver the request priority.</param>
        /// <returns>AdaptativeLimiterOptions</returns>
        public AdaptativeLimiterOptions UsePriorityResolver(Func<HttpContext, Task<Priority>> priorityResolverFunc)
            => this.UsePriorityResolver(new CustomPriorityResolver(priorityResolverFunc));

        /// <summary>
        /// Sets a custom priority resolver instance.
        /// </summary>
        /// <param name="priorityResolver">Priority resolver instance.</param>
        /// <returns>AdaptativeLimiterOptions</returns>
        public AdaptativeLimiterOptions UsePriorityResolver(IPriorityResolver priorityResolver)
        {
            this.PriorityResolver = priorityResolver;
            return this;
        }
    }
}
