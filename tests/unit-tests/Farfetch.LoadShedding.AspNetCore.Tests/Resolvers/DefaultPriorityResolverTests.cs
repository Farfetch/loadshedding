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

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultPriorityResolverTests"/> class.
        /// </summary>
        public DefaultPriorityResolverTests()
        {
            this._contextMock = new Mock<HttpContext>();
            this._target = new DefaultPriorityResolver();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ResolveAsync_WithDefaultPriority_ReturnsNormal()
        {
            // Act
            var result = await this._target.ResolveAsync(this._contextMock.Object);

            // Assert
            Assert.Equal(Priority.Normal, result);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
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
