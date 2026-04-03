using Domain.Primitives;

namespace Domain.Entities;

public sealed class AssetConfiguration : Entity
{
    public Guid InventoryId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal? StandardValue { get; private set; }
    public string? Location { get; private set; }

    public HardwareInventory Inventory { get; private set; } = null!;
    public Product Product { get; private set; } = null!;

    public AssetConfiguration(Guid inventoryId, Guid productId, int quantity, decimal? standardValue, string? location)
    {
        if (quantity < 1)
            throw new ArgumentException("Quantity must be at least 1.", nameof(quantity));

        InventoryId = inventoryId;
        ProductId = productId;
        Quantity = quantity;
        StandardValue = standardValue;
        Location = location;
    }
}
