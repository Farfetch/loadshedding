using Farfetch.LoadShedding.Calculators;
using Farfetch.LoadShedding.Configurations;
using Moq;
using Xunit;

namespace Farfetch.LoadShedding.Tests.Calculators
{
    public class GradientLimitCalculatorTests
    {
        private readonly Mock<IConcurrencyContext> _concurrencyContextMock;

        /// <summary>
        /// Initializes a new instance of the <see cref="GradientLimitCalculatorTests"/> class.
        /// </summary>
        public GradientLimitCalculatorTests()
        {
            this._concurrencyContextMock = new Mock<IConcurrencyContext>();

            this._concurrencyContextMock.Setup(x => x.AvgRTT).Returns(50);
            this._concurrencyContextMock.Setup(x => x.MinRTT).Returns(50);
        }

        [Fact]
        public void CalculateLimit_NumberOfExecutionsEqualsToZero_ReturnsTheCurrentLimit()
        {
            // Arrange
            const int ExpectedResult = 0;

            var options = new ConcurrencyOptions();

            var target = new GradientLimitCalculator(options);

            this._concurrencyContextMock.Setup(x => x.MaxConcurrencyLimit).Returns(ExpectedResult);

            // Act
            var result = target.CalculateLimit(this._concurrencyContextMock.Object);

            // Assert
            Assert.Equal(options.MinConcurrencyLimit, result);
        }

        [Theory]
        [InlineData(20, 1, 2, 0, 0.2, 20)] // When Calculated Gradient Is Less Than 0.5
        [InlineData(20, 1, 2, 0, 1.5, 20)] // When Gradient Is Greater Than 1 And Max Queue Size Greater Than Current Limit
        [InlineData(20, 2, 2, 0, 1.5, 20)] // When Gradient Is Greater Than 1 And Max Queue Size Less Than Current Limit
        [InlineData(10, 5, 1, 0, 0.8, 10)] // When Current Limit Is Greater Than New Limit And Min Limit Being The Max Value
        [InlineData(5, 5, 1, 0, 0.8, 2)] // When Current Limit Is Greater Than New Limit And Smooth Limit Being The Max Value
        [InlineData(5, 5, 1, 60, 0.8, 2)] // When Current Limit Is Greater Than New Limit And Previous Avg Rtt Is Greater Than Current Avg Rtt
        public void CalculateLimit_WithMultipleInputs_ReturnsLimitCorrectly(
            int expectedResult,
            int maxConcurrencyLimit,
            int currentQueueItems,
            int previousAvgRTT,
            double tolerance,
            int minConcurrencyLimit)
        {
            // Arrange
            this._concurrencyContextMock.Setup(x => x.AvgRTT).Returns(50);
            this._concurrencyContextMock.Setup(x => x.MinRTT).Returns(50);
            this._concurrencyContextMock.Setup(x => x.MaxConcurrencyLimit).Returns(maxConcurrencyLimit);
            this._concurrencyContextMock.Setup(x => x.MaxQueueSize).Returns(maxConcurrencyLimit);
            this._concurrencyContextMock.Setup(x => x.CurrentQueueCount).Returns(currentQueueItems);
            this._concurrencyContextMock.Setup(x => x.PreviousAvgRTT).Returns(previousAvgRTT);

            var target = new GradientLimitCalculator(new ConcurrencyOptions
            {
                Tolerance = tolerance,
                MinConcurrencyLimit = minConcurrencyLimit,
            });

            // Act
            var result = target.CalculateLimit(this._concurrencyContextMock.Object);

            // Assert
            Assert.Equal(expectedResult, result);
        }
    }
}
