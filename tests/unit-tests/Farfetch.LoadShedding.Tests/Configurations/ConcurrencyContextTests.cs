using Farfetch.LoadShedding.Configurations;
using Farfetch.LoadShedding.Events;
using Farfetch.LoadShedding.Measures;
using Farfetch.LoadShedding.Tasks;
using Moq;
using Xunit;

namespace Farfetch.LoadShedding.Tests.Configurations
{
    public class ConcurrencyContextTests
    {
        [Fact]
        public void ConcurrencyContext_Initialization_ShouldSetTheCorrectValues()
        {
            // Arrange
            const int MaxQueueSize = 20, MaxConcurrencyLimit = 10, CurrentQueueCount = 0;

            var events = new Mock<ILoadSheddingEvents>();

            var taskManager = new TaskManager(
                MaxConcurrencyLimit,
                MaxQueueSize,
                Timeout.Infinite,
                events.Object);

            var rtpMeasures  = new RTTMeasures(2);

            // Act
            var target = new ConcurrencyContext(taskManager, rtpMeasures);

            // Assert
            Assert.Equal(MaxQueueSize, target.MaxQueueSize);
            Assert.Equal(MaxConcurrencyLimit, target.MaxConcurrencyLimit);
            Assert.Equal(CurrentQueueCount, target.CurrentQueueCount);
        }
    }
}
