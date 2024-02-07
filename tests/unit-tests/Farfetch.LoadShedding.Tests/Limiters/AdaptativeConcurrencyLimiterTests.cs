using Farfetch.LoadShedding.Calculators;
using Farfetch.LoadShedding.Configurations;
using Farfetch.LoadShedding.Events;
using Farfetch.LoadShedding.Limiters;
using Moq;
using Xunit;

namespace Farfetch.LoadShedding.Tests.Limiters
{
    public class AdaptativeConcurrencyLimiterTests
    {
        [Fact]
        public async Task ExecuteAsync_WhenInvoked_TheFunctionIsCalled()
        {
            // Arrange
            var wasInvoked = false;
            var events = new LoadSheddingEvents();
            var limitCalculatorMock = new Mock<ILimitCalculator>();
            var queueSizeCalculatorMock = new Mock<IQueueSizeCalculator>();

            var target = new AdaptativeConcurrencyLimiter(new ConcurrencyOptions(), limitCalculatorMock.Object, queueSizeCalculatorMock.Object, events);

            // Act
            await target.ExecuteAsync(() =>
            {
                wasInvoked = true;
                return Task.CompletedTask;
            });

            // Assert
            Assert.True(wasInvoked);
        }
    }
}
