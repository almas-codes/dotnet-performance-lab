using System.Reflection;
using BenchmarkDotNet.Attributes;
using PerformanceLab.Abstractions.Catalog;

namespace PerformanceLab.Core.Validation;

public sealed record BenchmarkValidationIssue(string Code, string Message);

public static class BenchmarkCatalogValidator
{
    public static IReadOnlyList<BenchmarkValidationIssue> Validate(Assembly benchmarkAssembly, string repositoryRoot)
    {
        var issues = new List<BenchmarkValidationIssue>();
        var catalog = new Catalog.ReflectionBenchmarkCatalog(benchmarkAssembly);
        var scenarios = catalog.GetScenarios();

        if (scenarios.Count == 0)
        {
            issues.Add(new BenchmarkValidationIssue("catalog-empty", "No benchmark scenarios were discovered."));
            return issues;
        }

        var ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var scenario in scenarios)
        {
            ValidateScenarioMetadata(scenario, ids, issues);
            ValidateBenchmarkType(scenario, issues);
            ValidateReadme(scenario, repositoryRoot, issues);
        }

        return issues;
    }

    private static void ValidateScenarioMetadata(
        IBenchmarkScenario scenario,
        ISet<string> ids,
        ICollection<BenchmarkValidationIssue> issues)
    {
        if (string.IsNullOrWhiteSpace(scenario.Id))
        {
            issues.Add(new BenchmarkValidationIssue("missing-id", "A scenario is missing Id."));
            return;
        }

        if (!ids.Add(scenario.Id))
        {
            issues.Add(new BenchmarkValidationIssue("duplicate-id", $"Duplicate benchmark id '{scenario.Id}'."));
        }

        if (string.IsNullOrWhiteSpace(scenario.Category))
        {
            issues.Add(new BenchmarkValidationIssue("missing-category", $"{scenario.Id}: missing Category."));
        }

        if (string.IsNullOrWhiteSpace(scenario.Name))
        {
            issues.Add(new BenchmarkValidationIssue("missing-name", $"{scenario.Id}: missing Name."));
        }

        if (string.IsNullOrWhiteSpace(scenario.Description))
        {
            issues.Add(new BenchmarkValidationIssue("missing-description", $"{scenario.Id}: missing Description."));
        }

        if (string.IsNullOrWhiteSpace(scenario.Hypothesis))
        {
            issues.Add(new BenchmarkValidationIssue("missing-hypothesis", $"{scenario.Id}: missing Hypothesis."));
        }
    }

    private static void ValidateBenchmarkType(IBenchmarkScenario scenario, ICollection<BenchmarkValidationIssue> issues)
    {
        var benchmarkType = scenario.BenchmarkType;
        if (benchmarkType is null || !benchmarkType.IsClass)
        {
            issues.Add(new BenchmarkValidationIssue("invalid-benchmark-type", $"{scenario.Id}: invalid BenchmarkType."));
            return;
        }

        var benchmarkMethods = benchmarkType
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .Where(m => m.GetCustomAttribute<BenchmarkAttribute>() is not null)
            .ToList();

        if (benchmarkMethods.Count < 2)
        {
            issues.Add(new BenchmarkValidationIssue(
                "insufficient-variants",
                $"{scenario.Id}: expected at least two [Benchmark] methods, found {benchmarkMethods.Count}."));
        }

        if (!benchmarkMethods.Any(m => m.GetCustomAttribute<BenchmarkAttribute>()?.Baseline == true))
        {
            issues.Add(new BenchmarkValidationIssue(
                "missing-baseline",
                $"{scenario.Id}: no [Benchmark(Baseline = true)] variant defined."));
        }
    }

    private static void ValidateReadme(
        IBenchmarkScenario scenario,
        string repositoryRoot,
        ICollection<BenchmarkValidationIssue> issues)
    {
        var readmePath = Path.Combine(repositoryRoot, "benchmarks", scenario.Category, scenario.Id, "README.md");
        if (!File.Exists(readmePath))
        {
            issues.Add(new BenchmarkValidationIssue(
                "missing-readme",
                $"{scenario.Id}: missing README at benchmarks/{scenario.Category}/{scenario.Id}/README.md"));
        }
    }
}
