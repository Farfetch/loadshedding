using Farfetch.LoadShedding.Measures;
using Xunit;

namespace Farfetch.LoadShedding.Tests.Measures
{
    public class RTTMeasuresTests
    {
        [Fact]
        public void AddSample_WithOneExecution_SetTheCorrectValues()
        {
            // Arrange
            const long ExpectedTotalCount = 1, ExpectedAvgRTT = 50, ExpectedMinRtt = 50;

            var target = new RTTMeasures(tolerance: 2);

            // Act
            target.AddSample(durationInMs: 50);

            // Assert
            Assert.Equal(ExpectedTotalCount, target.TotalCount);
            Assert.Equal(ExpectedMinRtt, target.MinRTT);
            Assert.Equal(ExpectedAvgRTT, target.AvgRTT);
        }

        [Fact]
        public void AddSample_WithMultipleExecutions_SetTheCorrectValues()
        {
            // Arrange
            const long ExpectedTotalCount = 5, ExpectedAvgRTT = 44, ExpectedMinRtt = 40;

            var target = new RTTMeasures(tolerance: 1.5);

            // Act
            new double[] { 50, 30, 55, 60, 25 }.ToList().ForEach(x => target.AddSample(durationInMs: x));

            // Assert
            Assert.Equal(ExpectedTotalCount, target.TotalCount);
            Assert.Equal(ExpectedMinRtt, target.MinRTT);
            Assert.Equal(ExpectedAvgRTT, target.AvgRTT);
        }

        [Theory]
        [InlineData(1.5, 50)]
        [InlineData(0.8, 45)]
        public void RecoverFromLoad_WithDifferentTolerance_TheValuesAreOrNotSet(double tolerance, long expectedAvgRtt)
        {
            // Arrange
            var target = new RTTMeasures(tolerance);
            target.AddSample(durationInMs: 50);

            // Act
            target.RecoverFromLoad();

            // Assert
            Assert.Equal(expectedAvgRtt, target.AvgRTT);
        }
    }
}
