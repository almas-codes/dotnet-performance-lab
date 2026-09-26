using System.Collections.Generic;

namespace PerformanceLab.Abstractions.Catalog;

public interface IBenchmarkScenario
{
    string Id { get; }
    string Name { get; }
    string Category { get; }
    string Description { get; }
    string Hypothesis { get; }
    
    // Type of the BenchmarkDotNet class to run
    System.Type BenchmarkType { get; }
}

public interface IBenchmarkCatalog
{
    IReadOnlyList<IBenchmarkScenario> GetScenarios();
}
