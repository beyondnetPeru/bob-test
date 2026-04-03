using Domain.Entities;

namespace Application.Abstractions.Repositories;

public interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken cancellationToken = default);
}

public interface IRepository<T, TId> where T : class
{
    Task<T?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Delete(T entity);
}

public interface IManufacturerRepository : IRepository<Manufacturer, Guid>
{
}

public interface ICategoryRepository : IRepository<ProductCategory, Guid>
{
}

public interface IProductRepository : IRepository<Product, Guid>
{
}

public interface IInventoryRepository : IRepository<HardwareInventory, Guid>
{
}

public interface IAssetConfigurationRepository : IRepository<AssetConfiguration, Guid>
{
}
