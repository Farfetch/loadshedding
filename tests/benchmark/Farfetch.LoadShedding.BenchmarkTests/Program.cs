using BenchmarkDotNet.Running;
using Farfetch.LoadShedding.BenchmarkTests;

BenchmarkRunner.Run<AdaptativeLimiterBenchmarks>();
BenchmarkRunner.Run<TaskQueueBenchmarks>();
