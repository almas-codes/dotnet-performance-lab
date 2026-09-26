using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using PerformanceLab.Abstractions.Catalog;

namespace PerformanceLab.Benchmarks.Concurrency;

public class ChannelVsConcurrentQueueScenario : IBenchmarkScenario
{
    public string Id => "channel-vs-concurrent-queue";
    public string Name => "Channel vs ConcurrentQueue";
    public string Category => "Concurrency";
    public string Description => "Single producer / single consumer throughput for Channel<T> versus ConcurrentQueue<T>.";
    public string Hypothesis => "Channel<T> can provide cleaner async producer/consumer semantics with comparable throughput for bounded steady-state workloads.";
    public Type BenchmarkType => typeof(ChannelVsConcurrentQueueBenchmark);
}

[MemoryDiagnoser]
public class ChannelVsConcurrentQueueBenchmark
{
    private const int MessageCount = 10_000;

    [Benchmark(Baseline = true, Description = "ConcurrentQueue")]
    public int ConcurrentQueuePipeline()
    {
        var queue = new ConcurrentQueue<int>();
        var consumed = 0;

        for (var i = 0; i < MessageCount; i++)
        {
            queue.Enqueue(i);
        }

        while (queue.TryDequeue(out _))
        {
            consumed++;
        }

        return consumed;
    }

    [Benchmark(Description = "Channel (unbounded)")]
    public async Task<int> ChannelPipeline()
    {
        var channel = Channel.CreateUnbounded<int>();
        var writer = channel.Writer;
        var reader = channel.Reader;
        var consumed = 0;

        for (var i = 0; i < MessageCount; i++)
        {
            writer.TryWrite(i);
        }

        writer.Complete();

        await foreach (var _ in reader.ReadAllAsync())
        {
            consumed++;
        }

        return consumed;
    }
}
