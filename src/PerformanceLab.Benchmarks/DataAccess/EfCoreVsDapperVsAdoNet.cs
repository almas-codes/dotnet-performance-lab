using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using PerformanceLab.Abstractions.Catalog;
using PerformanceLab.Benchmarks.Infrastructure;
using PerformanceLab.Data;
using PerformanceLab.Data.EfCore;

namespace PerformanceLab.Benchmarks.DataAccess;

public class EfCoreVsDapperVsAdoNetScenario : IBenchmarkScenario
{
    public string Id => "efcore-vs-dapper-vs-adonet";
    public string Name => "EF Core vs Dapper vs ADO.NET";
    public string Category => "DataAccess";
    public string Description => "Compares reading 100, 1_000, and 10_000 rows across major ORMs/data access technologies.";
    public string Hypothesis => "ADO.NET will have the lowest allocation and latency. Dapper will be within 10-15% of ADO.NET. EF Core NoTracking will be within 30% of Dapper.";
    public Type BenchmarkType => typeof(EfCoreVsDapperVsAdoNetBenchmark);
}

[MemoryDiagnoser]
public class EfCoreVsDapperVsAdoNetBenchmark
{
    private string _connectionString = null!;
    private PerformanceDbContext _dbContext = null!;
    private NpgsqlConnection _connection = null!;

    private const string Sql = "SELECT \"Id\", \"Name\", \"Email\", \"CreatedAt\" FROM \"Customers\" ORDER BY \"Id\" LIMIT @Limit";

    [Params(100, 1_000, 10_000)]
    public int Limit { get; set; }

    [GlobalSetup]
    public async Task Setup()
    {
        _connectionString = PostgresBenchmarkFixture.ConnectionString;
        await PostgresBenchmarkFixture.EnsureCustomersSeededAsync(10_000);

        var options = new DbContextOptionsBuilder<PerformanceDbContext>()
            .UseNpgsql(_connectionString)
            .Options;

        _dbContext = new PerformanceDbContext(options);
        _connection = new NpgsqlConnection(_connectionString);
        await _connection.OpenAsync();
    }

    [GlobalCleanup]
    public async Task Cleanup()
    {
        await _connection.DisposeAsync();
        await _dbContext.DisposeAsync();
    }

    [Benchmark(Baseline = true, Description = "ADO.NET")]
    public async Task<List<Customer>> AdoNet()
    {
        var result = new List<Customer>(Limit);
        await using var cmd = new NpgsqlCommand(Sql, _connection);
        cmd.Parameters.AddWithValue("@Limit", Limit);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            result.Add(new Customer
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Email = reader.GetString(2),
                CreatedAt = reader.GetDateTime(3)
            });
        }

        return result;
    }

    [Benchmark(Description = "Dapper")]
    public async Task<List<Customer>> Dapper()
    {
        var result = await _connection.QueryAsync<Customer>(Sql, new { Limit });
        return result.AsList();
    }

    [Benchmark(Description = "EF Core (NoTracking)")]
    public async Task<List<Customer>> EfCoreNoTracking()
    {
        return await _dbContext.Customers
            .AsNoTracking()
            .OrderBy(c => c.Id)
            .Take(Limit)
            .ToListAsync();
    }

    [Benchmark(Description = "EF Core (Tracking)")]
    public async Task<List<Customer>> EfCoreTracking()
    {
        return await _dbContext.Customers
            .OrderBy(c => c.Id)
            .Take(Limit)
            .ToListAsync();
    }
}
