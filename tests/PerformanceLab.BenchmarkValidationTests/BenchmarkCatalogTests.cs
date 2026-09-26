using PerformanceLab.Benchmarks.Collections;
using PerformanceLab.Core.Catalog;
using PerformanceLab.Core.Validation;

namespace PerformanceLab.BenchmarkValidationTests;

public class BenchmarkCatalogTests
{
    private static string RepositoryRoot =>
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));

    [Fact]
    public void Catalog_Discovers_Flagship_Benchmarks()
    {
        var catalog = new ReflectionBenchmarkCatalog(typeof(ListVsDictionaryLookupScenario).Assembly);
        var scenarios = catalog.GetScenarios();

        Assert.Equal(6, scenarios.Count);
        Assert.Contains(scenarios, s => s.Id == "dictionary-vs-list");
        Assert.Contains(scenarios, s => s.Id == "efcore-vs-dapper-vs-adonet");
        Assert.Contains(scenarios, s => s.Id == "tracking-vs-no-tracking");
        Assert.Contains(scenarios, s => s.Id == "offset-vs-keyset");
        Assert.Contains(scenarios, s => s.Id == "reflection-vs-source-generated-json");
        Assert.Contains(scenarios, s => s.Id == "channel-vs-concurrent-queue");
    }

    [Fact]
    public void Validator_Passes_For_Repository_Benchmarks()
    {
        var issues = BenchmarkCatalogValidator.Validate(typeof(ListVsDictionaryLookupScenario).Assembly, RepositoryRoot);
        Assert.True(issues.Count == 0, string.Join(Environment.NewLine, issues.Select(i => i.Message)));
    }

    [Fact]
    public void Every_Scenario_Has_BenchmarkType_With_Baseline()
    {
        var catalog = new ReflectionBenchmarkCatalog(typeof(ListVsDictionaryLookupScenario).Assembly);

        foreach (var scenario in catalog.GetScenarios())
        {
            Assert.NotNull(scenario.BenchmarkType);
            Assert.False(string.IsNullOrWhiteSpace(scenario.Hypothesis));
        }
    }
}
