using PerformanceLab.Abstractions.Catalog;
using PerformanceLab.Benchmarks.Collections;

namespace PerformanceLab.BenchmarkValidationTests;

public class BenchmarkScenarioSmokeTests
{
    public static IEnumerable<object[]> ScenarioTypes =>
        typeof(ListVsDictionaryLookupScenario).Assembly
            .GetTypes()
            .Where(t => !t.IsAbstract && typeof(IBenchmarkScenario).IsAssignableFrom(t))
            .Select(t => new object[] { t });

    [Theory]
    [MemberData(nameof(ScenarioTypes))]
    public void Scenario_Can_Be_Instantiated_And_Provides_BenchmarkType(Type scenarioType)
    {
        var scenario = Activator.CreateInstance(scenarioType) as IBenchmarkScenario;
        Assert.NotNull(scenario);
        Assert.NotNull(scenario.BenchmarkType);
        Assert.True(scenario.BenchmarkType.IsClass);
    }
}
