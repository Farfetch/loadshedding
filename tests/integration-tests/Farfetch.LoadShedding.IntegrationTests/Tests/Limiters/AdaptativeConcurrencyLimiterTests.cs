using System.Collections.Concurrent;
using System.Net;
using Farfetch.LoadShedding.AspNetCore.Options;
using Farfetch.LoadShedding.Configurations;
using Farfetch.LoadShedding.Prometheus;
using Farfetch.LoadShedding.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;
using Xunit;
using Xunit.Abstractions;

namespace Farfetch.LoadShedding.IntegrationTests.Tests.Limiters
{
    public class AdaptativeConcurrencyLimiterTests
    {
        private readonly ConcurrentBag<int> _queueLimits = new();
        private readonly ConcurrentBag<int> _concurrencyLimits = new();
        private readonly ConcurrentBag<Priority> _enqueuedItems = new();
        private readonly CollectorRegistry _collectorRegistry;
        private readonly ITestOutputHelper output;

        private int _numberOfRejectedRequests;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdaptativeConcurrencyLimiterTests"/> class.
        /// </summary>
        public AdaptativeConcurrencyLimiterTests(ITestOutputHelper output)
        {
            this._numberOfRejectedRequests = 0;
            this._collectorRegistry = new CollectorRegistry();
            this.output = output;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task GetAsync_WithReducedLimitAndQueueSizeAndMultiplePrioritiesAndMethods_SomeRequestsAreRejected()
        {
            // Arrange
            const int InitialConcurrencyLimit = 100, InitialQueueSize = 4, MinSuccessfulRequests = 44;

            var options = new ConcurrencyOptions
            {
                QueueTimeoutInMs = 2,
                InitialConcurrencyLimit = InitialConcurrencyLimit,
                InitialQueueSize = InitialQueueSize,
                MinQueueSize = InitialQueueSize,
            };

            var client = this.GetClient(options, x => x.UseHeaderPriorityResolver());

            // Act
            var tasks = Enumerable
                .Range(0, 1000)
                .Select(i => Task.Run(() =>
                {
                    var message = new HttpRequestMessage(i % 3 == 0 ? HttpMethod.Delete : HttpMethod.Get, "/api/people");
                    message.Headers.Add("X-Priority", i % 2 == 0 ? "critical" : "normal");
                    return client.SendAsync(message);
                }));

            var results = await Task.WhenAll(tasks.ToArray());

            // Assert
            Assert.True(results.Count(x => x.IsSuccessStatusCode) >= MinSuccessfulRequests);
            Assert.Contains(results, x => x.StatusCode == HttpStatusCode.ServiceUnavailable);
            await AssertMetrics(client, labels: new[] { ("GET", "normal"), ("GET", "critical"), ("DELETE", "normal"), ("DELETE", "critical") }, hadQueueItems: true, hadRejectedItems: true);
        }

        [Fact]
        public async Task GetAsync_WithReducedLimitAndQueueSizeAndMultiplePriorities_SomeRequestsAreRejected()
        {
            // Arrange
            const int InitialConcurrencyLimit = 100, InitialQueueSize = 4, MinSuccessfulRequests = 44;

            var options = new ConcurrencyOptions
            {
                QueueTimeoutInMs = 2,
                InitialConcurrencyLimit = InitialConcurrencyLimit,
                InitialQueueSize = InitialQueueSize,
                MinQueueSize = InitialQueueSize,
            };

            var client = this.GetClient(options, x => x.UseHeaderPriorityResolver());

            // Act
            var tasks = Enumerable
                .Range(0, 1000)
                .Select(i => Task.Run(() =>
                {
                    var message = new HttpRequestMessage(HttpMethod.Get, "/api/people");
                    message.Headers.Add("X-Priority", ((Priority)(i % 3)).ToString().ToLower());
                    return client.SendAsync(message);
                }));

            var results = await Task.WhenAll(tasks.ToArray());

            // Assert
            Assert.True(results.Count(x => x.IsSuccessStatusCode) >= MinSuccessfulRequests);
            Assert.Contains(results, x => x.StatusCode == HttpStatusCode.ServiceUnavailable);

            await AssertMetrics(client, labels: new[] { ("GET", "critical"), ("GET", "normal"), ("GET", "noncritical") }, hadQueueItems: true, hadRejectedItems: true);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task GetAsync_WithHighLimitAndQueueSize_NoneRequestsIsRejected()
        {
            // Arrange
            const int InitialConcurrencyLimit = 160, InitialQueueSize = 4, ExpectedSuccessfulRequests = 160, ExpectedRejectedRequests = 0;

            var options = new ConcurrencyOptions
            {
                InitialConcurrencyLimit = InitialConcurrencyLimit,
                InitialQueueSize = InitialQueueSize,
                MinQueueSize = InitialQueueSize,
            };

            var client = this.GetClient(options);

            // Act
            var tasks = Enumerable
                .Range(0, 160)
                .Select(_ => Task.Run(() => client.GetAsync("/api/people")));

            var results = await Task.WhenAll(tasks.ToArray());

            // Assert
            Assert.Equal(ExpectedSuccessfulRequests, results.Count(x => x.IsSuccessStatusCode));
            Assert.Equal(ExpectedRejectedRequests, results.Count(x => x.StatusCode == HttpStatusCode.ServiceUnavailable));
            await AssertMetrics(client, priority: "normal", hadQueueItems: false, hadRejectedItems: false);
        }

        [Theory]
        [InlineData("critical", Priority.Critical)]
        [InlineData("normal", Priority.Normal)]
        [InlineData("noncritical", Priority.NonCritical)]
        [InlineData("non-critical", Priority.NonCritical)]
        [InlineData("other", Priority.Normal)]
        public async Task GetAsync_WithHeaderPriority_ResolvePriorityFromHeaderValue(string headerValue, Priority priority)
        {
            // Arrange
            var options = new ConcurrencyOptions
            {
                InitialConcurrencyLimit = 1,
                MinConcurrencyLimit = 1,
                MaxConcurrencyLimit = 2,
                InitialQueueSize = int.MaxValue,
                MinQueueSize = int.MaxValue,
            };

            var client = this.GetClient(options, x => x.UseHeaderPriorityResolver());

            client.DefaultRequestHeaders.Add("X-Priority", headerValue);

            // Act
            var tasks = Enumerable
                .Range(0, 20)
                .Select(_ => Task.Run(() => client.GetAsync("/api/people")));

            var results = await Task.WhenAll(tasks.ToArray());

            // Assert
            Assert.NotEmpty(_enqueuedItems);
            Assert.True(_enqueuedItems.All(x => x == priority));
            await AssertMetrics(client, priority.FormatPriority(), hadQueueItems: false, hadRejectedItems: false);
        }

        [Theory]
        [InlineData("critical")]
        [InlineData("normal")]
        [InlineData("noncritical")]
        [InlineData("non-critical")]
        [InlineData("other")]
        public async Task GetAsync_WithEndpointPriority_ResolveFromEndpointPriorityAttribute(string headerValue)
        {
            // Arrange
            var options = new ConcurrencyOptions
            {
                InitialConcurrencyLimit = 1,
                MinConcurrencyLimit = 1,
                MaxConcurrencyLimit = 2,
                InitialQueueSize = int.MaxValue,
                MinQueueSize = int.MaxValue,
            };

            var client = this.GetClient(options, x => x.UseEndpointPriorityResolver());

            client.DefaultRequestHeaders.Add("X-Priority", headerValue);

            // Act
            var tasks = Enumerable
                .Range(0, 5)
                .Select(_ => Task.Run(() => client.GetAsync("/api/people")));

            var results = await Task.WhenAll(tasks.ToArray());

            // Assert
            Assert.NotEmpty(_enqueuedItems);
            Assert.True(_enqueuedItems.All(x => x == Priority.Critical));
            await AssertMetrics(client, "critical", hadQueueItems: false, hadRejectedItems: false);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task GetAsync_WithListener_TheEventValuesAreCorrect()
        {
            // Arrange
            const int InitialConcurrencyLimit = 10, InitialQueueSize = 10, MinQueueSize = 4;

            var options = new ConcurrencyOptions
            {
                InitialConcurrencyLimit = InitialConcurrencyLimit,
                MinConcurrencyLimit = InitialConcurrencyLimit,
                InitialQueueSize = InitialQueueSize,
                MinQueueSize = MinQueueSize,
            };

            var client = this.GetClient(options);

            // Act
            var tasks = Enumerable
                .Range(0, 80)
                .Select(_ => client.GetAsync("/api/people"));

            var results = await Task.WhenAll(tasks.ToArray());

            // Assert
            Assert.Equal(_numberOfRejectedRequests, results.Count(x => x.StatusCode == HttpStatusCode.ServiceUnavailable));
            Assert.Contains(this._concurrencyLimits, x => x == InitialConcurrencyLimit);
            Assert.Contains(this._queueLimits, x => x == InitialQueueSize);
            await AssertMetrics(client, "normal", hadQueueItems: false, hadRejectedItems: false);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task GetMetrics_WithDisableMetrics_ShouldNotExportDisableMetrics()
        {
            // Arrange
            var client = this.GetClient(
                new ConcurrencyOptions(),
                metricOptionsDelegate: options =>
                {
                    options.QueueLimit.Enabled = false;
                    options.ConcurrencyLimit.Enabled = false;
                    options.RequestRejected.Enabled = false;
                });

            await client.GetAsync("/api/people");

            // Act
            var metrics = await client.GetAsync("/monitoring/metrics");

            // Assert
            Assert.Equal(HttpStatusCode.OK, metrics.StatusCode);
            Assert.NotNull(metrics.Content);
            Assert.Equal("text/plain", metrics.Content?.Headers?.ContentType?.MediaType);

            Assert.NotNull(metrics.Content);

            var content = await metrics.Content.ReadAsStringAsync();

            Assert.DoesNotContain("http_requests_concurrency_limit_total", content);
            Assert.DoesNotContain("http_requests_queue_limit_total", content);
            Assert.DoesNotContain("http_requests_rejected_total", content);

            Assert.Contains("http_requests_concurrency_items_total", content);
            Assert.Contains("http_requests_task_processing_time_seconds", content);
        }

        public HttpClient GetClient(
            ConcurrencyOptions concurrencyoptions,
            Action<AdaptativeLimiterOptions>? optionsDelegate = null,
            Action<LoadSheddingMetricOptions>? metricOptionsDelegate = null)
        {
            var testServer = new TestServer(
              new WebHostBuilder()
              .ConfigureServices(services =>
              {
                  services.AddLoadShedding((provider, options) =>
                  {
                      optionsDelegate?.Invoke(options.AdaptativeLimiter);

                      options.AdaptativeLimiter.ConcurrencyOptions.MinConcurrencyLimit = concurrencyoptions.MinConcurrencyLimit;
                      options.AdaptativeLimiter.ConcurrencyOptions.MaxConcurrencyLimit = concurrencyoptions.MaxConcurrencyLimit;
                      options.AdaptativeLimiter.ConcurrencyOptions.InitialConcurrencyLimit = concurrencyoptions.InitialConcurrencyLimit;
                      options.AdaptativeLimiter.ConcurrencyOptions.Tolerance = concurrencyoptions.Tolerance;
                      options.AdaptativeLimiter.ConcurrencyOptions.InitialQueueSize = concurrencyoptions.InitialQueueSize;
                      options.AdaptativeLimiter.ConcurrencyOptions.MinQueueSize = concurrencyoptions.MinQueueSize;

                      options.SubscribeEvents((p, events) =>
                      {
                          events.QueueLimitChanged.Subscribe(args => this._queueLimits.Add(args.Limit));
                          events.ConcurrencyLimitChanged.Subscribe(args => this._concurrencyLimits.Add(args.Limit));
                          events.Rejected.Subscribe(args => Interlocked.Increment(ref this._numberOfRejectedRequests));
                          events.ItemEnqueued.Subscribe(args => this._enqueuedItems.Add(args.Priority));
                      });

                      options.AddMetrics(options =>
                      {
                          options.Registry = this._collectorRegistry;
                          metricOptionsDelegate?.Invoke(options);
                      });
                  })
                  .AddControllers();
              })
              .Configure(x => x
                .UseMetricServer("/monitoring/metrics", this._collectorRegistry)
                .UseRouting()
                .UseLoadShedding()
                .UseEndpoints(endpoints => endpoints.MapControllers())));

            return testServer.CreateClient();
        }

        private Task AssertMetrics(HttpClient client, string priority, bool hadQueueItems, bool hadRejectedItems)
        {
            return AssertMetrics(client, new[] { ("GET", priority) }, hadQueueItems, hadRejectedItems);
        }

        private async Task AssertMetrics(HttpClient client, (string method, string priority)[] labels, bool hadQueueItems, bool hadRejectedItems)
        {
            var metrics = await client.GetAsync("/monitoring/metrics");

            // Assert
            Assert.Equal(HttpStatusCode.OK, metrics.StatusCode);
            Assert.NotNull(metrics.Content);
            Assert.Equal("text/plain", metrics.Content?.Headers?.ContentType?.MediaType);

            var content = metrics.Content?.ReadAsStringAsync().GetAwaiter().GetResult();
            output.WriteLine(content);

            // Assert Concurrency
            Assert.Contains("TYPE http_requests_concurrency_items_total gauge", content);
            Assert.Contains("TYPE http_requests_concurrency_limit_total gauge", content);
            foreach (var (method, priority) in labels)
            {
                Assert.Contains($"http_requests_concurrency_items_total{{method=\"{method}\",priority=\"{priority}\"}} 0", content);
            }

            Assert.Contains("http_requests_concurrency_limit_total", content);

            // Assert Processing
            Assert.Contains("TYPE http_requests_task_processing_time_seconds histogram", content);
            foreach (var (method, priority) in labels)
            {
                Assert.Contains($"http_requests_task_processing_time_seconds_sum{{method=\"{method}\",priority=\"{priority}\"}}", content);
                Assert.Contains($"http_requests_task_processing_time_seconds_count{{method=\"{method}\",priority=\"{priority}\"}}", content);
                Assert.Contains($"http_requests_task_processing_time_seconds_bucket{{method=\"{method}\",priority=\"{priority}\",le=\"+Inf\"}}", content);
            }

            // Assert Queue
            Assert.Contains("TYPE http_requests_queue_items_total gauge", content);
            Assert.Contains("TYPE http_requests_queue_time_seconds histogram", content);
            if (hadQueueItems)
            {
                Assert.Contains("http_requests_queue_limit_total", content);
                foreach (var (method, priority) in labels)
                {
                    Assert.Contains($"http_requests_queue_items_total{{method=\"{method}\",priority=\"{priority}\"}} 0", content);
                    Assert.Contains($"http_requests_queue_time_seconds_sum{{method=\"{method}\",priority=\"{priority}\"}}", content);
                    Assert.Contains($"http_requests_queue_time_seconds_count{{method=\"{method}\",priority=\"{priority}\"}}", content);
                    Assert.Contains($"http_requests_queue_time_seconds_bucket{{method=\"{method}\",priority=\"{priority}\",le=\"+Inf\"}}", content);
                }
            }

            // Assert Rejection
            Assert.Contains("TYPE http_requests_rejected_total counter", content);
            if (hadRejectedItems)
            {
                foreach (var (method, priority) in labels)
                {
                    Assert.Contains($"http_requests_rejected_total{{method=\"{method}\",priority=\"{priority}\",reason=\"max_queue_items\"}}", content);
                }
            }
        }
    }
}
