using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public sealed class ManufacturerConfiguration : IEntityTypeConfiguration<Manufacturer>
{
    public void Configure(EntityTypeBuilder<Manufacturer> builder)
    {
        builder.HasKey(m => m.Id);
        
        builder.Property(m => m.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(m => m.Name)
            .IsUnique();
    }
}
