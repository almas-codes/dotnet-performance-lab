using PerformanceLab.Reporting;

namespace PerformanceLab.Reporting.Tests;

public class LatestRunSummaryWriterTests
{
    [Fact]
    public void ReadEntries_Returns_Results_From_Existing_Artifacts()
    {
        var repositoryRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
        var resultsDirectory = Path.Combine(repositoryRoot, "reports", "latest", "raw", "results");

        if (!Directory.Exists(resultsDirectory))
        {
            return;
        }

        var entries = LatestRunSummaryWriter.ReadEntries(resultsDirectory);
        Assert.NotEmpty(entries);
    }

    [Fact]
    public void WriteMarkdownSummary_Creates_File()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "perflab-report-test-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);

        try
        {
            LatestRunSummaryWriter.WriteMarkdownSummary(tempRoot, new[]
            {
                new BenchmarkRunEntry("DemoBenchmark", "MethodA", "Size=10", 12.5, 64, 1.2)
            });

            var summaryPath = Path.Combine(tempRoot, "reports", "latest", "summary.md");
            Assert.True(File.Exists(summaryPath));
            var content = File.ReadAllText(summaryPath);
            Assert.Contains("DemoBenchmark", content);
        }
        finally
        {
            Directory.Delete(tempRoot, recursive: true);
        }
    }
}
