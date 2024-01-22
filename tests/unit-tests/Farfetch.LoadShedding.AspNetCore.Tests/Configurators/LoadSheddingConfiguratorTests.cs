using Farfetch.LoadShedding.AspNetCore.Configurators;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Farfetch.LoadShedding.AspNetCore.Tests.Configurators
{
    public class LoadSheddingConfiguratorTests
    {
        [Fact]
        public void LoadSheddingConfigurator_UseAdaptativeLimiterWithOptions_RegisterOptionsWithCorrectValues()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddLoadShedding((provider, options) =>
            {
                options.AdaptativeLimiter.ConcurrencyOptions.MinConcurrencyLimit = 1;
            });

            // Assert
            var options = services
                .BuildServiceProvider()
                .GetService<LoadSheddingOptions>();

            Assert.NotNull(options);
            Assert.Equal(1, options.AdaptativeLimiter.ConcurrencyOptions.MinConcurrencyLimit);
        }
    }
}
