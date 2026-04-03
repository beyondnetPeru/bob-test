using Domain.Primitives;

namespace Domain.Entities;

public sealed class Product : Entity
{
    public Guid CategoryId { get; private set; }
    public Guid ManufacturerId { get; private set; }
    public string ModelName { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    public ProductCategory Category { get; private set; } = null!;
    public Manufacturer Manufacturer { get; private set; } = null!;
    public ICollection<AssetConfiguration> AssetConfigurations { get; private set; } = new List<AssetConfiguration>();

    public Product(Guid categoryId, Guid manufacturerId, string modelName, string description)
    {
        if (categoryId == Guid.Empty) throw new ArgumentException("CategoryId cannot be empty.", nameof(categoryId));
        if (manufacturerId == Guid.Empty) throw new ArgumentException("ManufacturerId cannot be empty.", nameof(manufacturerId));
        ArgumentException.ThrowIfNullOrWhiteSpace(modelName);

        CategoryId = categoryId;
        ManufacturerId = manufacturerId;
        ModelName = modelName;
        Description = description;
    }

    public void Update(Guid categoryId, Guid manufacturerId, string modelName, string description)
    {
        if (categoryId == Guid.Empty) throw new ArgumentException("CategoryId cannot be empty.", nameof(categoryId));
        if (manufacturerId == Guid.Empty) throw new ArgumentException("ManufacturerId cannot be empty.", nameof(manufacturerId));
        ArgumentException.ThrowIfNullOrWhiteSpace(modelName);

        CategoryId = categoryId;
        ManufacturerId = manufacturerId;
        ModelName = modelName;
        Description = description;
    }
}
