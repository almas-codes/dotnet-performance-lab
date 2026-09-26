using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Json;
using BenchmarkDotNet.Jobs;

namespace PerformanceLab.Core.Configuration;

public class PerformanceLabBenchmarkConfig : ManualConfig
{
    public PerformanceLabBenchmarkConfig()
    {
        // Default to a diagnostic-friendly setup
        AddDiagnoser(MemoryDiagnoser.Default);

        // Standard exporter suite
        AddExporter(MarkdownExporter.GitHub);
        AddExporter(JsonExporter.Full);
        
        // Define standard job - strictly .NET 10
        var job = Job.Default
            .WithStrategy(RunStrategy.Throughput)
            .WithWarmupCount(3)
            .WithIterationCount(10);
            
        AddJob(job);

        // Keep artifacts organized
        ArtifactsPath = System.IO.Path.Combine(System.Environment.CurrentDirectory, "reports", "latest", "raw");
    }
}
