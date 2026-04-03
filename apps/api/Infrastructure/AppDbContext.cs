using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Infrastructure.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Manufacturer> Manufacturers => Set<Manufacturer>();
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<HardwareInventory> HardwareInventories => Set<HardwareInventory>();
    public DbSet<AssetConfiguration> AssetConfigurations => Set<AssetConfiguration>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Automatically Apply all IEntityTypeConfiguration in the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified);

        foreach (var entityEntry in entries)
        {
            // Only set audit timestamps on entities that have those properties in the model
            var entityType = entityEntry.Metadata;

            if (entityType.FindProperty("UpdatedAt") != null)
            {
                entityEntry.Property("UpdatedAt").CurrentValue = DateTimeOffset.UtcNow;
            }

            if (entityEntry.State == EntityState.Added && entityType.FindProperty("CreatedAt") != null)
            {
                entityEntry.Property("CreatedAt").CurrentValue = DateTimeOffset.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
