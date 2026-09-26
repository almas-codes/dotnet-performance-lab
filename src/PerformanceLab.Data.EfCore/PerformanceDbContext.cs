using Microsoft.EntityFrameworkCore;
using PerformanceLab.Data;

namespace PerformanceLab.Data.EfCore;

public class PerformanceDbContext : DbContext
{
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    
    public PerformanceDbContext(DbContextOptions<PerformanceDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(b =>
        {
            b.HasKey(c => c.Id);
            b.Property(c => c.Name).HasMaxLength(100);
            b.Property(c => c.Email).HasMaxLength(255);
            b.HasIndex(c => c.Email).IsUnique();
        });

        modelBuilder.Entity<Order>(b =>
        {
            b.HasKey(o => o.Id);
            b.Property(o => o.Status).HasMaxLength(50);
            b.HasIndex(o => o.CustomerId);
            b.HasIndex(o => o.OrderDate);
            b.HasIndex(o => new { o.OrderDate, o.Id });

            b.HasOne(o => o.Customer)
             .WithMany(c => c.Orders)
             .HasForeignKey(o => o.CustomerId);
        });
    }
}
