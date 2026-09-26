using Microsoft.EntityFrameworkCore;
using Npgsql;
using PerformanceLab.Data;
using PerformanceLab.Data.EfCore;

namespace PerformanceLab.Benchmarks.Infrastructure;

public static class PostgresBenchmarkFixture
{
    public const string DefaultConnectionString =
        "Host=localhost;Port=5432;Database=perf_lab;Username=postgres;Password=perf_lab_dev";

    public static string ConnectionString =>
        global::System.Environment.GetEnvironmentVariable("PERFLAB_POSTGRES") ?? DefaultConnectionString;

    public static bool IsAvailable()
    {
        try
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            connection.Open();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static async Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync(cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static void EnsureAvailable()
    {
        if (!IsAvailable())
        {
            throw new InvalidOperationException(
                "PostgreSQL is not available. Start it with ./infrastructure/scripts/start-databases.ps1 " +
                "or set PERFLAB_POSTGRES to a reachable connection string.");
        }
    }

    public static PerformanceDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<PerformanceDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        return new PerformanceDbContext(options);
    }

    public static async Task EnsureCustomersSeededAsync(int customerCount, CancellationToken cancellationToken = default)
    {
        EnsureAvailable();
        await using var db = CreateContext();
        await db.Database.EnsureCreatedAsync(cancellationToken);

        if (await db.Customers.AsNoTracking().CountAsync(cancellationToken) >= customerCount)
        {
            return;
        }

        var existing = await db.Customers.CountAsync(cancellationToken);
        var customers = new List<Customer>(customerCount - existing);
        for (var i = existing; i < customerCount; i++)
        {
            customers.Add(new Customer
            {
                Name = $"Customer {i}",
                Email = $"customer{i}@example.com",
                CreatedAt = DateTime.UtcNow.AddMinutes(-i)
            });
        }

        await db.Customers.AddRangeAsync(customers, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    public static async Task EnsureOrdersSeededAsync(int orderCount, CancellationToken cancellationToken = default)
    {
        EnsureAvailable();
        await using var db = CreateContext();
        await db.Database.EnsureCreatedAsync(cancellationToken);

        if (await db.Orders.AsNoTracking().CountAsync(cancellationToken) >= orderCount)
        {
            return;
        }

        var customerIds = await db.Customers.AsNoTracking()
            .OrderBy(c => c.Id)
            .Select(c => c.Id)
            .Take(Math.Max(1, orderCount / 10))
            .ToListAsync(cancellationToken);

        if (customerIds.Count == 0)
        {
            await EnsureCustomersSeededAsync(Math.Max(100, orderCount / 100), cancellationToken);
            customerIds = await db.Customers.AsNoTracking().Select(c => c.Id).Take(100).ToListAsync(cancellationToken);
        }

        var existing = await db.Orders.CountAsync(cancellationToken);
        var random = new Random(42);
        var orders = new List<Order>(orderCount - existing);

        for (var i = existing; i < orderCount; i++)
        {
            orders.Add(new Order
            {
                CustomerId = customerIds[i % customerIds.Count],
                TotalAmount = random.Next(10, 500),
                OrderDate = DateTime.UtcNow.AddDays(-(i % 3650)).AddSeconds(i),
                Status = i % 5 == 0 ? "Cancelled" : "Completed"
            });
        }

        await db.Orders.AddRangeAsync(orders, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }
}
