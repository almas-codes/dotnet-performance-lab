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

namespace PerformanceLab.Benchmarks.Pagination;

public class OffsetVsKeysetPaginationScenario : IBenchmarkScenario
{
    public string Id => "offset-vs-keyset";
    public string Name => "Offset vs Keyset Pagination";
    public string Category => "Pagination";
    public string Description => "Compares OFFSET/LIMIT pagination against keyset pagination at increasing page depths.";
    public string Hypothesis => "Keyset pagination maintains more stable latency as page depth increases because OFFSET must scan skipped rows.";
    public Type BenchmarkType => typeof(OffsetVsKeysetPaginationBenchmark);
}

[MemoryDiagnoser]
public class OffsetVsKeysetPaginationBenchmark
{
    private PerformanceDbContext _dbContext = null!;
    private const int PageSize = 50;
    private DateTime _keysetDate;
    private int _keysetId;

    [Params(1, 10, 100, 1_000)]
    public int Page { get; set; }

    [GlobalSetup]
    public async Task Setup()
    {
        await PostgresBenchmarkFixture.EnsureCustomersSeededAsync(1_000);
        await PostgresBenchmarkFixture.EnsureOrdersSeededAsync(100_000);

        var options = new DbContextOptionsBuilder<PerformanceDbContext>()
            .UseNpgsql(PostgresBenchmarkFixture.ConnectionString)
            .Options;

        _dbContext = new PerformanceDbContext(options);

        var anchor = await _dbContext.Orders.AsNoTracking()
            .OrderBy(o => o.OrderDate)
            .ThenBy(o => o.Id)
            .Skip((Page - 1) * PageSize)
            .Take(1)
            .Select(o => new { o.OrderDate, o.Id })
            .FirstAsync();

        _keysetDate = anchor.OrderDate;
        _keysetId = anchor.Id;
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

    [Benchmark(Baseline = true, Description = "Offset pagination")]
    public async Task<List<Order>> OffsetPagination()
    {
        var skip = (Page - 1) * PageSize;
        return await _dbContext.Orders.AsNoTracking()
            .OrderBy(o => o.OrderDate)
            .ThenBy(o => o.Id)
            .Skip(skip)
            .Take(PageSize)
            .ToListAsync();
    }

    [Benchmark(Description = "Keyset pagination")]
    public async Task<List<Order>> KeysetPagination()
    {
        return await _dbContext.Orders.AsNoTracking()
            .Where(o => o.OrderDate > _keysetDate || (o.OrderDate == _keysetDate && o.Id >= _keysetId))
            .OrderBy(o => o.OrderDate)
            .ThenBy(o => o.Id)
            .Take(PageSize)
            .ToListAsync();
    }
}
