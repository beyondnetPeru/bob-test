using AutoMapper;
using Domain.Entities;
using Application.Features.Manufacturers.Queries.GetManufacturerList;
using Application.Features.Categories.Queries.GetCategoryList;
using Application.Features.Products.Queries.GetProductList;
using Application.Features.Inventory.Queries.GetInventoryList;

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
            .ForMember(dest => dest.ComponentCount, opt => opt.MapFrom(src => src.AssetConfigurations.Count));
    }
}
