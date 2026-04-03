using Application.Abstractions.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class ManufacturerRepository(AppDbContext context) : EfRepository<Manufacturer, Guid>(context), IManufacturerRepository
{
}

public sealed class CategoryRepository(AppDbContext context) : EfRepository<ProductCategory, Guid>(context), ICategoryRepository
{
}

public sealed class ProductRepository(AppDbContext context) : EfRepository<Product, Guid>(context), IProductRepository
{
    public override Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.Products
            .Include(p => p.Category)
            .Include(p => p.Manufacturer)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public override Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default) =>
        _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Manufacturer)
            .ToListAsync(cancellationToken);
}

public sealed class InventoryRepository(AppDbContext context) : EfRepository<HardwareInventory, Guid>(context), IInventoryRepository
{
    public override Task<HardwareInventory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.HardwareInventories
            .Include(h => h.AssetConfigurations)
            .ThenInclude(ac => ac.Product)
            .ThenInclude(p => p.Category)
            .FirstOrDefaultAsync(h => h.Id == id, cancellationToken);

    public override Task<List<HardwareInventory>> GetAllAsync(CancellationToken cancellationToken = default) =>
        _context.HardwareInventories
            .AsNoTracking()
            .Include(h => h.AssetConfigurations)
            .ThenInclude(ac => ac.Product)
            .ThenInclude(p => p.Category)
            .ToListAsync(cancellationToken);
}
