using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public sealed class HardwareInventoryConfiguration : IEntityTypeConfiguration<HardwareInventory>
{
    public void Configure(EntityTypeBuilder<HardwareInventory> builder)
    {
        builder.HasKey(h => h.Id);
        
        builder.Property(h => h.AssetName)
            .HasMaxLength(255);
            
        builder.Property(h => h.WeightKg)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(h => h.DeviceCategory)
            .HasMaxLength(80)
            .IsRequired();
            
        builder.Property(h => h.PerformanceTier)
            .HasMaxLength(50);

        // Audit Shadow Properties
        builder.Property<DateTimeOffset>("CreatedAt")
            .IsRequired();
            
        builder.Property<DateTimeOffset>("UpdatedAt")
            .IsRequired();

        builder.Property<byte[]>("RowVersion")
            .IsRowVersion();

        builder.HasMany(h => h.AssetConfigurations)
            .WithOne(a => a.Inventory)
            .HasForeignKey(a => a.InventoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
