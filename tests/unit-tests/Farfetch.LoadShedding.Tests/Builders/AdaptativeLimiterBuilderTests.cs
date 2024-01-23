using Farfetch.LoadShedding.Builders;
using Xunit;

namespace Farfetch.LoadShedding.Tests.Builders
{
    public class AdaptativeLimiterBuilderTests
    {
        [Fact]
        public void Build_WithUseAdaptativeLimiter_LimiterShouldNotBeNull()
        {
            // Act
            var target = new AdaptativeLimiterBuilder()
                .WithOptions(options =>
                {
                    options.MinConcurrencyLimit = 20;
                    options.InitialConcurrencyLimit = 50;
                    options.InitialQueueSize = 50;
                    options.Tolerance = 2;
                })
                .Build();

            // Assert
            Assert.NotNull(target);
        }
    }
}
