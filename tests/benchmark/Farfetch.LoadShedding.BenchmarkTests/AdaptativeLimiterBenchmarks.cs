using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Farfetch.LoadShedding.AspNetCore.Middlewares;
using Farfetch.LoadShedding.AspNetCore.Resolvers;
using Farfetch.LoadShedding.Builders;
using Farfetch.LoadShedding.Limiters;
using Farfetch.LoadShedding.Tasks;
using Microsoft.AspNetCore.Http;

namespace Farfetch.LoadShedding.BenchmarkTests
{
    [MemoryDiagnoser]
    [ThreadingDiagnoser]
    [Orderer(SummaryOrderPolicy.Declared)]
    [MinColumn]
    [MaxColumn]
    [IterationCount(10)]
    [RankColumn]
    public class AdaptativeLimiterBenchmarks
    {
        private static readonly IPriorityResolver s_randomResolver = new CustomPriorityResolver(_ => Task.FromResult((Priority)Random.Shared.Next(3)));

        private static readonly HttpContext s_context = new DefaultHttpContext();

        private static readonly IAdaptativeConcurrencyLimiter s_limiter = new AdaptativeLimiterBuilder()
            .WithOptions(options => options.MaxConcurrencyLimit = int.MaxValue)
            .Build();

        private static readonly AdaptativeConcurrencyLimiterMiddleware s_middleware = new(context => Task.CompletedTask, s_limiter, null);

        private static readonly AdaptativeConcurrencyLimiterMiddleware s_middlewareWithRandomPriority = new(context => Task.CompletedTask, s_limiter, s_randomResolver);

        [Benchmark]
        public async Task Limiter_Default() => await s_limiter.ExecuteAsync(() => Task.CompletedTask);

        [Benchmark]
        public async Task Limiter_RandomPriority() => await s_limiter.ExecuteAsync((Priority)Random.Shared.Next(3), () => Task.CompletedTask);

        [Benchmark]
        public async Task LimiterMiddleware_Default() => await s_middleware.InvokeAsync(s_context);

        [Benchmark]
        public async Task LimiterMiddleware_RandomPriority() => await s_middlewareWithRandomPriority.InvokeAsync(s_context);
    }
}
