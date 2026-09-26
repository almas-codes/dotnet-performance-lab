using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PerformanceLab.Abstractions.Catalog;

namespace PerformanceLab.Core.Catalog;

public class ReflectionBenchmarkCatalog : IBenchmarkCatalog
{
    private readonly Lazy<IReadOnlyList<IBenchmarkScenario>> _scenarios;

    public ReflectionBenchmarkCatalog(Assembly assemblyToScan)
    {
        _scenarios = new Lazy<IReadOnlyList<IBenchmarkScenario>>(() =>
        {
            var scenarioType = typeof(IBenchmarkScenario);
            var types = assemblyToScan.GetTypes()
                .Where(t => !t.IsInterface && !t.IsAbstract && scenarioType.IsAssignableFrom(t));

            var instances = new List<IBenchmarkScenario>();
            foreach (var type in types)
            {
                if (Activator.CreateInstance(type) is IBenchmarkScenario instance)
                {
                    instances.Add(instance);
                }
            }

            return instances.OrderBy(x => x.Category).ThenBy(x => x.Name).ToList();
        });
    }

    public IReadOnlyList<IBenchmarkScenario> GetScenarios()
    {
        return _scenarios.Value;
    }
}
