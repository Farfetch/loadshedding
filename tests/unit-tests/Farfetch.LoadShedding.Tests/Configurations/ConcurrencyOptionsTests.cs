using Farfetch.LoadShedding.Configurations;
using Farfetch.LoadShedding.Exceptions;
using Xunit;

namespace Farfetch.LoadShedding.Tests.Configurations
{
    public class ConcurrencyOptionsTests
    {
        private const string ExpectedExceptionMessage = "The value of MinConcurrencyLimit, InitialConcurrencyLimit, MaxConcurrencyLimit, " +
            "MinQueueSize, or InitialQueueSize should be greater than 0";

        [Theory]
        [InlineData(20, 50, 500, 1.5, 20, 50)]
        [InlineData(1, 1, 2, 1.1, 1, 1)]
        public void Validate_WithValidConfigurations_NoExceptionIsThrown(
            int minConcurrencyLimit,
            int initialConcurrencyLimit,
            int maxConcurrencyLimit,
            double tolerance,
            int minQueueSize,
            int initialQueueSize)
        {
            // Arrange
            var target = new ConcurrencyOptions
            {
                MinConcurrencyLimit = minConcurrencyLimit,
                InitialConcurrencyLimit = initialConcurrencyLimit,
                MaxConcurrencyLimit = maxConcurrencyLimit,
                Tolerance = tolerance,
                MinQueueSize = minQueueSize,
                InitialQueueSize = initialQueueSize,
            };

            // Act
            var exception = Record.Exception(() => target.Validate());

            // Assert
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(0, 50, 500, 1.5, 20, 50, ExpectedExceptionMessage)]
        [InlineData(20, -5, 500, 1.5, 20, 50, ExpectedExceptionMessage)]
        [InlineData(20, 50, 0, 1.5, 20, 50, ExpectedExceptionMessage)]
        [InlineData(20, 50, 500, 1.5, 0, 50, ExpectedExceptionMessage)]
        [InlineData(20, 50, 500, 1.5, 20, 0, ExpectedExceptionMessage)]
        [InlineData(20, 50, 500, 1, 20, 50, "The value of Tolerance should be greater than 1")]
        [InlineData(20, 50, 10, 1.5, 20, 50, "The value of MaxConcurrencyLimit should be greater than the MinConcurrencyLimit")]
        [InlineData(20, 5, 500, 1.5, 20, 50, "The value of InitialConcurrencyLimit should be greater than MinConcurrencyLimit and less than MaxConcurrencyLimit")]
        [InlineData(20, 600, 500, 1.5, 20, 50, "The value of InitialConcurrencyLimit should be greater than MinConcurrencyLimit and less than MaxConcurrencyLimit")]
        [InlineData(20, 50, 500, 1.5, 20, 5, "The value of InitialQueueSize should be greater than the MinQueueSize")]
        public void Validate_WithInvalidConfigurations_ExceptionIsThrown(
            int minConcurrencyLimit,
            int initialConcurrencyLimit,
            int maxConcurrencyLimit,
            double tolerance,
            int minQueueSize,
            int initialQueueSize,
            string expectedMessage)
        {
            // Arrange
            var target = new ConcurrencyOptions
            {
                MinConcurrencyLimit = minConcurrencyLimit,
                InitialConcurrencyLimit = initialConcurrencyLimit,
                MaxConcurrencyLimit = maxConcurrencyLimit,
                Tolerance = tolerance,
                MinQueueSize = minQueueSize,
                InitialQueueSize = initialQueueSize,
            };

            // Act
            var exception = Assert.Throws<InvalidConfigurationException>(() => target.Validate());

            // Assert
            Assert.Equal(expectedMessage, exception.Message);
        }
    }
}
