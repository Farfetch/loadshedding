using Farfetch.LoadShedding.AspNetCore.Configurators;
using Farfetch.LoadShedding.AspNetCore.Middlewares;
using Farfetch.LoadShedding.AspNetCore.Resolvers;
using Farfetch.LoadShedding.Builders;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Extension to IApplicationBuilder in order to easily configure the UseLoadShedding.
    /// </summary>
    public static class IApplicationBuilderExtensions
    {
        /// <summary>
        /// Extension to add LoadShedding middlewares to the request pipeline.
        /// </summary>
        /// <param name="appBuilder">The IApplicationBuilder instance.</param>
        /// <returns>IApplicationBuilder</returns>
        public static IApplicationBuilder UseLoadShedding(this IApplicationBuilder appBuilder)
        {
            var options = appBuilder.ApplicationServices.GetService<LoadSheddingOptions>();

            if (options == null)
            {
                options = new LoadSheddingOptions();
            }

            var adaptativeLimiter = new AdaptativeLimiterBuilder()
                .WithOptions(options.AdaptativeLimiter.ConcurrencyOptions)
                .WithCustomQueueSizeCalculator(options.QueueSizeCalculator)
                .SubscribeEvents(events =>
                {
                    foreach (var listener in options.EventSubscriptions)
                    {
                        listener.Invoke(appBuilder.ApplicationServices, events);
                    }
                })
                .Build();

            appBuilder.UseMiddleware<AdaptativeConcurrencyLimiterMiddleware>(
                adaptativeLimiter,
                options.AdaptativeLimiter.PriorityResolver ?? new DefaultPriorityResolver());

            return appBuilder;
        }
    }
}
