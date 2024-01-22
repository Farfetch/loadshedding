using Farfetch.LoadShedding.AspNetCore.Resolvers;
using Farfetch.LoadShedding.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace Farfetch.LoadShedding.AspNetCore.Tests.Resolvers
{
    public class DefaultPriorityResolverTests
    {
        private readonly Mock<HttpContext> _contextMock;
        private readonly DefaultPriorityResolver _target;

        public DefaultPriorityResolverTests()
        {
            this._contextMock = new Mock<HttpContext>();
            this._target = new DefaultPriorityResolver();
        }

        [Fact]
        public async Task ResolveAsync_WithDefaultPriority_ReturnsNormal()
        {
            // Act
            var result = await this._target.ResolveAsync(this._contextMock.Object);

            // Assert
            Assert.Equal(Priority.Normal, result);
        }

        [Fact]
        public async Task ResolveAsync_WithDefaultPriorityMultipleTimes_AlwaysReturnNormal()
        {
            // Act
            var tasks = Enumerable.Range(0, 10).Select(_ => this._target.ResolveAsync(this._contextMock.Object));

            var results = await Task.WhenAll(tasks);

            // Assert
            Assert.True(results.All(x => x == Priority.Normal));
        }
    }
}
