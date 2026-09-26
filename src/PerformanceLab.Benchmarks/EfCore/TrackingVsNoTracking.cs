using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using PerformanceLab.Abstractions.Catalog;
using PerformanceLab.Benchmarks.Infrastructure;
using PerformanceLab.Data;
using PerformanceLab.Data.EfCore;

namespace PerformanceLab.Benchmarks.EfCore;

public class TrackingVsNoTrackingScenario : IBenchmarkScenario
{
    public string Id => "tracking-vs-no-tracking";
    public string Name => "EF Core Tracking vs NoTracking";
    public string Category => "EfCore";
    public string Description => "Repeated read-only queries with and without change tracking on the same shaped result set.";
    public string Hypothesis => "AsNoTracking reduces application-side overhead for read-only workloads that do not participate in updates.";
    public Type BenchmarkType => typeof(TrackingVsNoTrackingBenchmark);
}

[MemoryDiagnoser]
public class TrackingVsNoTrackingBenchmark
{
    private PerformanceDbContext _dbContext = null!;

    [Params(100, 1_000, 5_000)]
    public int Limit { get; set; }

    [GlobalSetup]
    public async Task Setup()
    {
        await PostgresBenchmarkFixture.EnsureCustomersSeededAsync(10_000);

        var options = new DbContextOptionsBuilder<PerformanceDbContext>()
            .UseNpgsql(PostgresBenchmarkFixture.ConnectionString)
            .Options;

        _dbContext = new PerformanceDbContext(options);
    }

    [GlobalCleanup]
    public async Task Cleanup()
    {
        await _dbContext.DisposeAsync();
    }

    [IterationSetup]
    public void ResetChangeTracker()
    {
        _dbContext.ChangeTracker.Clear();
    }

    [Benchmark(Baseline = true, Description = "AsNoTracking")]
    public async Task<List<Customer>> NoTracking()
    {
        return await _dbContext.Customers
            .AsNoTracking()
            .OrderBy(c => c.Id)
            .Take(Limit)
            .ToListAsync();
    }

    [Benchmark(Description = "Tracking")]
    public async Task<List<Customer>> Tracking()
    {
        return await _dbContext.Customers
            .OrderBy(c => c.Id)
            .Take(Limit)
            .ToListAsync();
    }
}
