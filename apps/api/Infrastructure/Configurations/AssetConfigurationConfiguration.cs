using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public sealed class AssetConfigurationConfiguration : IEntityTypeConfiguration<AssetConfiguration>
{
    public void Configure(EntityTypeBuilder<AssetConfiguration> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Quantity)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(a => a.StandardValue)
            .HasPrecision(18, 4);

        builder.Property(a => a.Location)
            .HasMaxLength(100);

        builder.HasOne(a => a.Inventory)
            .WithMany(i => i.AssetConfigurations)
            .HasForeignKey(a => a.InventoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Product)
            .WithMany(p => p.AssetConfigurations)
            .HasForeignKey(a => a.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
