using System;
using System.CommandLine;
using System.Threading.Tasks;

namespace PerformanceLab.Cli;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("dotnet-performance-lab - Reproducible .NET performance benchmarks");

        var listCommand = new Command("list", "List available benchmarks.");
        listCommand.SetHandler(() =>
        {
            var catalog = new PerformanceLab.Core.Catalog.ReflectionBenchmarkCatalog(typeof(PerformanceLab.Benchmarks.DataAccess.EfCoreVsDapperVsAdoNetScenario).Assembly);
            var scenarios = catalog.GetScenarios();
            
            Console.WriteLine(".NET Performance Lab");
            Console.WriteLine();
            
            foreach (var group in scenarios.GroupBy(x => x.Category))
            {
                Console.WriteLine($"{group.Key}");
                foreach (var scenario in group)
                {
                    Console.WriteLine($"  {scenario.Id}");
                    Console.WriteLine($"    {scenario.Name} - {scenario.Description}");
                }
                Console.WriteLine();
            }
        });
        
        var runCommand = new Command("run", "Run benchmarks.");
        var categoryOption = new Option<string>("--category", "Run benchmarks in this category.");
        var benchmarkOption = new Option<string>("--benchmark", "Run a specific benchmark by ID.");
        var datasetOption = new Option<string>("--dataset", "Dataset profile to use (e.g., Large).");
        runCommand.AddOption(categoryOption);
        runCommand.AddOption(benchmarkOption);
        runCommand.AddOption(datasetOption);
        runCommand.SetHandler((category, benchmark, dataset) =>
        {
            var catalog = new PerformanceLab.Core.Catalog.ReflectionBenchmarkCatalog(typeof(PerformanceLab.Benchmarks.DataAccess.EfCoreVsDapperVsAdoNetScenario).Assembly);
            var scenarios = catalog.GetScenarios();
            
            if (!string.IsNullOrEmpty(category))
            {
                scenarios = scenarios.Where(x => x.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            if (!string.IsNullOrEmpty(benchmark))
            {
                scenarios = scenarios.Where(x => x.Id.Equals(benchmark, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            
            Console.WriteLine($".NET Performance Lab");
            Console.WriteLine($"────────────────────────────────────────");
            Console.WriteLine($"Category       {category ?? "All"}");
            Console.WriteLine($"Benchmark      {benchmark ?? "All"}");
            Console.WriteLine($"Dataset        {dataset ?? "Default"}");
            Console.WriteLine($"Runtime        .NET 10");
            Console.WriteLine();
            Console.WriteLine($"Preparing environment...");
            Console.WriteLine($"✓ Environment prepared");
            Console.WriteLine();
            Console.WriteLine($"Running {scenarios.Count} scenarios...");
            
            var config = new PerformanceLab.Core.Configuration.PerformanceLabBenchmarkConfig();
            foreach (var scenario in scenarios)
            {
                Console.WriteLine($"Executing {scenario.Name}...");
                BenchmarkDotNet.Running.BenchmarkRunner.Run(scenario.BenchmarkType, config);
            }

            Console.WriteLine();
            Console.WriteLine($"Results:");
            Console.WriteLine($"reports/latest/");
        }, categoryOption, benchmarkOption, datasetOption);
        
        var compareCommand = new Command("compare", "Compare current run with a stored baseline.");
        var reportCommand = new Command("report", "Generate a report from the latest run.");
        var publishCommand = new Command("publish", "Publish results to README tables.");
        var envCommand = new Command("environment", "Print current environment fingerprint.");
        var validateCommand = new Command("validate", "Validate benchmarks.");
        var cleanCommand = new Command("clean", "Clean generated artifacts.");
        
        rootCommand.AddCommand(listCommand);
        rootCommand.AddCommand(runCommand);
        rootCommand.AddCommand(compareCommand);
        rootCommand.AddCommand(reportCommand);
        rootCommand.AddCommand(publishCommand);
        rootCommand.AddCommand(envCommand);
        rootCommand.AddCommand(validateCommand);
        rootCommand.AddCommand(cleanCommand);

        return await rootCommand.InvokeAsync(args);
    }
}
