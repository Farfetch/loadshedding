using Farfetch.LoadShedding.Tasks;
using Xunit;

namespace Farfetch.LoadShedding.Tests.Tasks
{
    public class PriorityExtensionsTests
    {
        [Theory]
        [InlineData("Critical", Priority.Critical)]
        [InlineData("critical", Priority.Critical)]
        [InlineData("criti-cal", Priority.Critical)]
        [InlineData("critical-", Priority.Critical)]
        [InlineData("-critical", Priority.Critical)]
        [InlineData("--critical", Priority.Critical)]
        [InlineData("NORMAL", Priority.Normal)]
        [InlineData("normal", Priority.Normal)]
        [InlineData("nor-mal", Priority.Normal)]
        [InlineData("nor--mal", Priority.Normal)]
        [InlineData("normal-", Priority.Normal)]
        [InlineData("-normal", Priority.Normal)]
        [InlineData("noncritical", Priority.NonCritical)]
        [InlineData("non-critical", Priority.NonCritical)]
        [InlineData("NON-critical", Priority.NonCritical)]
        [InlineData("noncritical-", Priority.NonCritical)]
        [InlineData("noncritical--", Priority.NonCritical)]
        [InlineData("-NONCRITICAL", Priority.NonCritical)]
        [InlineData("", Priority.Normal)]
        [InlineData(null, Priority.Normal)]
        [InlineData("other", Priority.Normal)]
        public void ParsePriority(string? value, Priority priority)
        {
            // Act
            var result = PriorityExtensions.ParsePriority(value);

            // Assert
            Assert.Equal(priority, result);
        }

        [Theory]
        [InlineData(Priority.Critical, "critical")]
        [InlineData(Priority.Normal, "normal")]
        [InlineData(Priority.NonCritical, "noncritical")]
        public void FormatPriority(Priority priority, string value)
        {
            // Act
            var result = PriorityExtensions.FormatPriority(priority);

            // Assert
            Assert.Equal(value, result);
        }
    }
}
