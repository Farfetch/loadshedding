using Farfetch.LoadShedding.Calculators;
using Farfetch.LoadShedding.Configurations;
using Moq;
using Xunit;

namespace Farfetch.LoadShedding.Tests.Calculators
{
    public class SquareRootQueueCalculatorTests
    {
        [Theory]
        [InlineData(10, 1, 10)]
        [InlineData(10, 1028, 33)]
        [InlineData(10, 20, 10)]
        [InlineData(1, 20, 5)]
        [InlineData(10, 121, 11)]
        public void CalculateQueueSize_WithMultipleInputs_ReturnsTheQueueValueCorrectly(int minQueueSize, int maxConcurrencyLimit, int expectedResult)
        {
            // Arrange
            var contextMock = new Mock<IConcurrencyContext>();
            contextMock.Setup(x => x.MaxConcurrencyLimit).Returns(maxConcurrencyLimit);

            var options = new ConcurrencyOptions { MinQueueSize = minQueueSize };

            var target = new SquareRootQueueCalculator(options);

            // Act
            var result = target.CalculateQueueSize(contextMock.Object);

            // Assert
            Assert.Equal(expectedResult, result);
        }
    }
}
