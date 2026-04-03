using System.ComponentModel.DataAnnotations;

namespace Application.Features.AssetConfigurations.Commands.Requests;

public sealed record CreateAssetConfigurationRequest(
    [Required(ErrorMessage = "Inventory identifier is required")]
    Guid InventoryId,
    [Required(ErrorMessage = "Product identifier is required")]
    Guid ProductId,
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    int Quantity,
    decimal? StandardValue,
    [StringLength(255, ErrorMessage = "Location cannot exceed 255 characters")]
    string? Location
);

public sealed record UpdateAssetConfigurationRequest(
    [Required]
    Guid Id,
    [Required(ErrorMessage = "Inventory identifier is required")]
    Guid InventoryId,
    [Required(ErrorMessage = "Product identifier is required")]
    Guid ProductId,
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    int Quantity,
    decimal? StandardValue,
    [StringLength(255, ErrorMessage = "Location cannot exceed 255 characters")]
    string? Location
);
