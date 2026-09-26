using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Json;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;

namespace PerformanceLab.Core.Configuration;

public class PerformanceLabBenchmarkConfig : ManualConfig
{
    public PerformanceLabBenchmarkConfig(BenchmarkRunMode mode = BenchmarkRunMode.Standard)
    {
        AddLogger(ConsoleLogger.Default);
        AddColumnProvider(DefaultColumnProviders.Instance);
        AddDiagnoser(MemoryDiagnoser.Default);
        AddExporter(MarkdownExporter.GitHub);
        AddExporter(JsonExporter.Full);

        var job = mode switch
        {
            BenchmarkRunMode.Quick => Job.Default
                .WithStrategy(RunStrategy.Throughput)
                .WithWarmupCount(1)
                .WithIterationCount(3),
            BenchmarkRunMode.Standard => Job.Default
                .WithStrategy(RunStrategy.Throughput)
                .WithWarmupCount(3)
                .WithIterationCount(10),
            BenchmarkRunMode.Deep => Job.Default
                .WithStrategy(RunStrategy.Throughput)
                .WithWarmupCount(5)
                .WithIterationCount(20),
            _ => Job.Default
        };

        AddJob(job);
        ArtifactsPath = Path.Combine(System.Environment.CurrentDirectory, "reports", "latest", "raw");
    }
}

public enum BenchmarkRunMode
{
    Quick,
    Standard,
    Deep
}
