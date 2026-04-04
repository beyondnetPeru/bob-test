using AutoMapper;
using Domain.Entities;
using Application.Features.Manufacturers.Queries.GetManufacturerList;
using Application.Features.Categories.Queries.GetCategoryList;
using Application.Features.Products.Queries.GetProductList;
using Application.Features.Inventory.Queries.GetInventoryList;
using Application.Features.AssetConfigurations.Queries.GetAssetConfigurationList;

namespace Application.Common.Mappings;

public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Manufacturer Mapping
        CreateMap<Manufacturer, ManufacturerDto>();

        // Category Mapping
        CreateMap<ProductCategory, CategoryDto>();

        // Product Mapping
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.ManufacturerName, opt => opt.MapFrom(src => src.Manufacturer.Name));

        // Inventory Mapping
        CreateMap<HardwareInventory, InventoryListDto>()
            .ForMember(dest => dest.ComponentCount, opt => opt.MapFrom(src => src.AssetConfigurations.Sum(configuration => configuration.Quantity)))
            .ForMember(dest => dest.DeviceCategory, opt => opt.MapFrom(src => src.DeviceCategory))
            .ForMember(dest => dest.Cpu, opt => opt.MapFrom(src => GetFirstComponentName(src, "CPU")))
            .ForMember(dest => dest.Ram, opt => opt.MapFrom(src => GetFirstComponentName(src, "RAM")))
            .ForMember(dest => dest.HardDisc, opt => opt.MapFrom(src => GetFirstComponentName(src, "STORAGE")))
            .ForMember(dest => dest.Video, opt => opt.MapFrom(src => GetFirstComponentName(src, "GPU")))
            .ForMember(dest => dest.PowerSource, opt => opt.MapFrom(src => GetFirstComponentName(src, "PSU")))
            .ForMember(dest => dest.Ports, opt => opt.MapFrom(src => GetPortsSummary(src)));

        // Asset Configuration Mapping
        CreateMap<AssetConfiguration, AssetConfigurationDto>()
            .ForMember(dest => dest.InventoryName, opt => opt.MapFrom(src => src.Inventory.AssetName))
            .ForMember(dest => dest.ProductModelName, opt => opt.MapFrom(src => src.Product.ModelName))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Product.Category.Name));
    }

    private static string GetFirstComponentName(HardwareInventory inventory, string categoryName)
    {
        return inventory.AssetConfigurations
            .Where(configuration => string.Equals(configuration.Product?.Category?.Name, categoryName, StringComparison.OrdinalIgnoreCase))
            .OrderBy(configuration => configuration.Location)
            .Select(configuration => configuration.Product.ModelName)
            .FirstOrDefault() ?? "-";
    }

    private static string GetPortsSummary(HardwareInventory inventory)
    {
        var ports = inventory.AssetConfigurations
            .Where(configuration => string.Equals(configuration.Product?.Category?.Name, "PORT", StringComparison.OrdinalIgnoreCase))
            .OrderBy(configuration => configuration.Product.ModelName)
            .Select(configuration => $"{configuration.Quantity} x {configuration.Product.ModelName}")
            .ToList();

        return ports.Count == 0 ? "-" : string.Join(", ", ports);
    }
}
