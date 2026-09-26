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
            Console.WriteLine(".NET Performance Lab");
            Console.WriteLine();
            Console.WriteLine("Data Access");
            Console.WriteLine("  efcore-vs-dapper-vs-adonet");
            Console.WriteLine("  tracking-vs-no-tracking");
            Console.WriteLine("  compiled-vs-normal-query");
            Console.WriteLine();
            Console.WriteLine("Collections");
            Console.WriteLine("  dictionary-vs-list");
            Console.WriteLine("  hashset-vs-list");
            Console.WriteLine();
            // Just a mock for now
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
            Console.WriteLine($"Running benchmarks...");
            
            // Run actual benchmarks for proof-of-concept
            BenchmarkDotNet.Running.BenchmarkRunner.Run<PerformanceLab.Benchmarks.DataAccess.EfCoreVsDapperVsAdoNetBenchmark>(
                new PerformanceLab.Core.Configuration.PerformanceLabBenchmarkConfig());
            
            BenchmarkDotNet.Running.BenchmarkRunner.Run<PerformanceLab.Benchmarks.Collections.ListVsDictionaryLookupBenchmark>(
                new PerformanceLab.Core.Configuration.PerformanceLabBenchmarkConfig());

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
