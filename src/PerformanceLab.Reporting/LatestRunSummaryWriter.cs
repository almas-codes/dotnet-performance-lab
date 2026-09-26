using System.Text.Json;

namespace PerformanceLab.Reporting;

public sealed record BenchmarkRunEntry(
    string Benchmark,
    string Method,
    string Parameters,
    double MeanNs,
    double AllocatedBytes,
    double StandardDeviationNs);

public static class LatestRunSummaryWriter
{
    public static IReadOnlyList<BenchmarkRunEntry> ReadEntries(string rawResultsDirectory)
    {
        if (!Directory.Exists(rawResultsDirectory))
        {
            return Array.Empty<BenchmarkRunEntry>();
        }

        var entries = new List<BenchmarkRunEntry>();
        foreach (var file in Directory.EnumerateFiles(rawResultsDirectory, "*-report-full.json", SearchOption.AllDirectories))
        {
            using var stream = File.OpenRead(file);
            using var document = JsonDocument.Parse(stream);
            if (!document.RootElement.TryGetProperty("Benchmarks", out var benchmarks))
            {
                continue;
            }

            foreach (var benchmark in benchmarks.EnumerateArray())
            {
                var statistics = benchmark.GetProperty("Statistics");
                var memory = benchmark.TryGetProperty("Memory", out var memoryElement)
                    ? memoryElement
                    : default;

                entries.Add(new BenchmarkRunEntry(
                    Benchmark: benchmark.GetProperty("Type").GetString() ?? "unknown",
                    Method: benchmark.GetProperty("MethodTitle").GetString()
                        ?? benchmark.GetProperty("Method").GetString()
                        ?? "unknown",
                    Parameters: benchmark.TryGetProperty("Parameters", out var parameters)
                        ? parameters.GetString() ?? string.Empty
                        : string.Empty,
                    MeanNs: statistics.GetProperty("Mean").GetDouble(),
                    AllocatedBytes: memory.ValueKind == JsonValueKind.Object
                        ? memory.GetProperty("BytesAllocatedPerOperation").GetDouble()
                        : 0,
                    StandardDeviationNs: statistics.GetProperty("StandardDeviation").GetDouble()));
            }
        }

        return entries;
    }

    public static void WriteMarkdownSummary(string repositoryRoot, IReadOnlyList<BenchmarkRunEntry> entries)
    {
        var summaryPath = Path.Combine(repositoryRoot, "reports", "latest", "summary.md");
        Directory.CreateDirectory(Path.GetDirectoryName(summaryPath)!);

        using var writer = new StreamWriter(summaryPath);
        writer.WriteLine("# Latest Benchmark Results");
        writer.WriteLine();
        writer.WriteLine($"Generated: {DateTime.UtcNow:O}");
        writer.WriteLine();

        if (entries.Count == 0)
        {
            writer.WriteLine("No benchmark results found. Run:");
            writer.WriteLine();
            writer.WriteLine("```bash");
            writer.WriteLine("dotnet run --project src/PerformanceLab.Cli -- run --benchmark dictionary-vs-list --quick");
            writer.WriteLine("```");
            return;
        }

        writer.WriteLine("| Benchmark | Method | Parameters | Mean (ns) | Allocated (B/op) | StdDev (ns) |");
        writer.WriteLine("|---|---|---|---:|---:|---:|");

        foreach (var entry in entries.OrderBy(e => e.Benchmark).ThenBy(e => e.Method))
        {
            writer.WriteLine(
                $"| `{entry.Benchmark}` | {entry.Method} | {entry.Parameters} | {entry.MeanNs:N2} | {entry.AllocatedBytes:N0} | {entry.StandardDeviationNs:N2} |");
        }
    }
}
