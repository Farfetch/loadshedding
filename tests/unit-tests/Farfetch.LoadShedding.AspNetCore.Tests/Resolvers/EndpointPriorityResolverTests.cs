using Farfetch.LoadShedding.AspNetCore.Attributes;
using Farfetch.LoadShedding.AspNetCore.Resolvers;
using Farfetch.LoadShedding.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Moq;
using Xunit;

namespace Farfetch.LoadShedding.AspNetCore.Tests.Resolvers
{
    public class EndpointPriorityResolverTests
    {
        private readonly Mock<HttpContext> _contextMock;
        private readonly Mock<IEndpointFeature> _endpointFeatureMock;
        private readonly EndpointPriorityResolver _target;

        public EndpointPriorityResolverTests()
        {
            this._contextMock = new Mock<HttpContext>();
            this._endpointFeatureMock = new Mock<IEndpointFeature>();
            this._target = new EndpointPriorityResolver();

            var featuresMock = new Mock<IFeatureCollection>();

            featuresMock.Setup(x => x.Get<IEndpointFeature>()).Returns(() => this._endpointFeatureMock.Object);

            this._contextMock.Setup(x => x.Features).Returns(() => featuresMock.Object);
        }

        [Theory]
        [InlineData(Priority.Critical)]
        [InlineData(Priority.NonCritical)]
        [InlineData(Priority.Normal)]
        public async Task ResolveAsync_WithEndpointPriority_ReturnPriority(Priority priority)
        {
            // Arrange
            var metadataCollection = new EndpointMetadataCollection(new EndpointPriorityAttribute(priority));

            var endpoint = new Endpoint(null, metadataCollection, string.Empty);

            _endpointFeatureMock.Setup(x => x.Endpoint).Returns(endpoint);

            // Act
            var result = await this._target.ResolveAsync(this._contextMock.Object);

            // Assert
            Assert.Equal(priority, result);
        }

        [Fact]
        public async Task ResolveAsync_MissingEndpointAttribute_ReturnPriority()
        {
            // Act
            var result = await this._target.ResolveAsync(this._contextMock.Object);

            // Assert
            Assert.Equal(Priority.Normal, result);
        }
    }
}
