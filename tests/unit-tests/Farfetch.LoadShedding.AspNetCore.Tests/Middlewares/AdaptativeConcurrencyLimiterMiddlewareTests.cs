using Farfetch.LoadShedding.AspNetCore.Middlewares;
using Farfetch.LoadShedding.AspNetCore.Options;
using Farfetch.LoadShedding.Exceptions;
using Farfetch.LoadShedding.Limiters;
using Farfetch.LoadShedding.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace Farfetch.LoadShedding.AspNetCore.Tests.Middlewares
{
    public class AdaptativeConcurrencyLimiterMiddlewareTests
    {
        private readonly Mock<HttpContext> _contextMock;
        private readonly Mock<HttpResponse> _responseMock;
        private readonly Mock<HttpRequest> _requestMock;
        private readonly Mock<RequestDelegate> _requestDelegateMock;
        private readonly Mock<IAdaptativeConcurrencyLimiter> _concurrencyLimiterMock;
        private readonly AdaptativeLimiterOptions _options;
        private readonly HeaderDictionary _headers;

        public AdaptativeConcurrencyLimiterMiddlewareTests()
        {
            this._contextMock = new Mock<HttpContext>();
            this._responseMock = new Mock<HttpResponse>();
            this._requestMock = new Mock<HttpRequest>();
            this._requestDelegateMock = new Mock<RequestDelegate>();
            this._concurrencyLimiterMock = new Mock<IAdaptativeConcurrencyLimiter>();
            this._options = new AdaptativeLimiterOptions();

            this._headers = new HeaderDictionary();

            this._requestMock.Setup(x => x.Headers).Returns(() => this._headers);
            this._requestMock.Setup(x => x.Method).Returns(HttpMethod.Get.Method);

            this._contextMock.SetupGet(x => x.Request).Returns(() => this._requestMock.Object);
            this._contextMock.SetupGet(x => x.Response).Returns(() => this._responseMock.Object);
        }

        [Theory]
        [InlineData("critical", Priority.Critical)]
        [InlineData("noncritical", Priority.NonCritical)]
        [InlineData("non-critical", Priority.NonCritical)]
        [InlineData("normal", Priority.Normal)]
        [InlineData("other", Priority.Normal)]
        public async Task InvokeAsync_WithHeaderPriority_ExecuteAsyncIsInvokedWithDefinedPriority(string headerValue, Priority priority)
        {
            // Arrange
            const string headerName = "X-Priority-Test";

            this._concurrencyLimiterMock
                .Setup(x => x.ExecuteAsync(It.IsAny<Func<Task>>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(() => Task.CompletedTask);

            this._requestDelegateMock
                .Setup(x => x.Invoke(It.IsAny<HttpContext>()))
                .Returns(() => Task.CompletedTask);

            this._options.UseHeaderPriorityResolver(headerName);

            this._headers.Add(headerName, headerValue);

            var target = new AdaptativeConcurrencyLimiterMiddleware(
                this._requestDelegateMock.Object,
                this._concurrencyLimiterMock.Object,
                this._options.PriorityResolver);

            // Act
            await target.InvokeAsync(this._contextMock.Object);

            // Assert
            this._concurrencyLimiterMock.Verify(x => x.ExecuteAsync(priority, It.IsAny<Func<Task>>(), It.IsAny<string>(), It.IsAny<CancellationToken>()));
            this._responseMock.VerifySet(x => x.StatusCode = It.IsAny<int>(), Times.Never);
        }

        [Fact]
        public async Task InvokeAsync_WithoutException_ExecuteAsyncIsInvokedWithDefaultPriority()
        {
            // Arrange
            this._concurrencyLimiterMock
                .Setup(x => x.ExecuteAsync(It.IsAny<Priority>(), It.IsAny<Func<Task>>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(() => Task.CompletedTask);

            this._requestDelegateMock
                .Setup(x => x.Invoke(It.IsAny<HttpContext>()))
                .Returns(() => Task.CompletedTask);

            var target = new AdaptativeConcurrencyLimiterMiddleware(
                this._requestDelegateMock.Object,
                this._concurrencyLimiterMock.Object,
                this._options.PriorityResolver);

            // Act
            await target.InvokeAsync(this._contextMock.Object);

            // Assert
            this._concurrencyLimiterMock.Verify(x => x.ExecuteAsync(Priority.Normal, It.IsAny<Func<Task>>(), It.IsAny<string>(), It.IsAny<CancellationToken>()));
            this._responseMock.VerifySet(x => x.StatusCode = It.IsAny<int>(), Times.Never);
        }

        [Fact]
        public async Task InvokeAsync_WithLimitReachedExceptionException_Returns()
        {
            // Arrange
            this._concurrencyLimiterMock
                .Setup(x => x.ExecuteAsync(It.IsAny<Priority>(), It.IsAny<Func<Task>>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Throws(new LimitReachedException("Exception"));

            var target = new AdaptativeConcurrencyLimiterMiddleware(
                this._requestDelegateMock.Object,
                this._concurrencyLimiterMock.Object,
                this._options.PriorityResolver);

            // Act
            await target.InvokeAsync(this._contextMock.Object);

            // Assert
            this._responseMock.VerifySet(x => x.StatusCode = StatusCodes.Status503ServiceUnavailable);
        }
    }
}
