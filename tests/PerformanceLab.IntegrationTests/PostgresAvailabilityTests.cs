using PerformanceLab.Benchmarks.Infrastructure;

namespace PerformanceLab.IntegrationTests;

public class PostgresAvailabilityTests
{
    [Fact]
    public async Task Postgres_Availability_Is_Detectable()
    {
        _ = await PostgresBenchmarkFixture.IsAvailableAsync();
    }

    [Fact]
    public async Task Postgres_Can_Seed_Customers_When_Available()
    {
        if (!await PostgresBenchmarkFixture.IsAvailableAsync())
        {
            return;
        }

        await PostgresBenchmarkFixture.EnsureCustomersSeededAsync(100);

        await using var db = PostgresBenchmarkFixture.CreateContext();
        var count = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.CountAsync(db.Customers);
        Assert.True(count >= 100);
    }
}
