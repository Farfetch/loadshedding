using Farfetch.LoadShedding.AspNetCore.Configurators;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Extension to IServiceCollection in order to easily configure the AddLoadShedding.
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Extension to configure the LoadShedding options.
        /// </summary>
        /// <param name="services">The IServiceCollection instance.</param>
        /// <param name="configDelegate">The configuration action.</param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddLoadShedding(
            this IServiceCollection services,
            Action<IServiceProvider, LoadSheddingOptions> configDelegate)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton(provider =>
            {
                var configurator = new LoadSheddingOptions();

                configDelegate?.Invoke(provider, configurator);

                return configurator;
            });

            return services;
        }

        /// <summary>
        /// Extension to configure the LoadShedding options.
        /// </summary>
        /// <param name="services">The IServiceCollection instance.</param>
        /// <param name="configDelegate">The configuration action.</param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddLoadShedding(this IServiceCollection services, Action<LoadSheddingOptions> configDelegate)
        {
            return services.AddLoadShedding((_, options) => configDelegate?.Invoke(options));
        }

        /// <summary>
        /// Extension to configure the LoadShedding options.
        /// </summary>
        /// <param name="services">The IServiceCollection instance.</param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddLoadShedding(this IServiceCollection services)
        {
            return services.AddLoadShedding(_ => { });
        }
    }
}
