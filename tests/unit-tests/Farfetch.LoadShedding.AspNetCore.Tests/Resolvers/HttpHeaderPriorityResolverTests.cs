using Farfetch.LoadShedding.AspNetCore.Resolvers;
using Farfetch.LoadShedding.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace Farfetch.LoadShedding.AspNetCore.Tests.Resolvers
{
    public class HttpHeaderPriorityResolverTests
    {
        private readonly Mock<HttpContext> _contextMock;
        private readonly HeaderDictionary _headers;

        public HttpHeaderPriorityResolverTests()
        {
            this._contextMock = new Mock<HttpContext>();
            this._headers = new HeaderDictionary();

            var requestMock = new Mock<HttpRequest>();

            requestMock.Setup(x => x.Headers).Returns(() => this._headers);

            this._contextMock.SetupGet(x => x.Request).Returns(() => requestMock.Object);
        }

        [Theory]
        [InlineData("critical", Priority.Critical)]
        [InlineData("noncritical", Priority.NonCritical)]
        [InlineData("non-critical", Priority.NonCritical)]
        [InlineData("normal", Priority.Normal)]
        [InlineData("other", Priority.Normal)]
        public async Task ResolveAsync_WithHeaderPriority_ReturnPriority(string headerValue, Priority priority)
        {
            // Arrange
            var target = new HttpHeaderPriorityResolver();

            this._headers[HttpHeaderPriorityResolver.DefaultPriorityHeaderName] = headerValue;

            // Act
            var result = await target.ResolveAsync(this._contextMock.Object);

            // Assert
            Assert.Equal(priority, result);
        }
    }
}
