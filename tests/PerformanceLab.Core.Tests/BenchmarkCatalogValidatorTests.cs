using PerformanceLab.Core.Configuration;
using PerformanceLab.Core.Validation;
using PerformanceLab.Benchmarks.Collections;

namespace PerformanceLab.Core.Tests;

public class BenchmarkCatalogValidatorTests
{
    private static string RepositoryRoot =>
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));

    [Fact]
    public void Quick_Config_Creates_Successfully()
    {
        var quick = new PerformanceLabBenchmarkConfig(BenchmarkRunMode.Quick);
        var standard = new PerformanceLabBenchmarkConfig(BenchmarkRunMode.Standard);

        Assert.NotNull(quick);
        Assert.NotNull(standard);
    }

    [Fact]
    public void Validator_Finds_No_Issues_In_Repository()
    {
        var issues = BenchmarkCatalogValidator.Validate(typeof(ListVsDictionaryLookupScenario).Assembly, RepositoryRoot);
        Assert.Empty(issues);
    }
}
