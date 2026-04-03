using System.ComponentModel.DataAnnotations;

namespace Application.Features.Products.Commands.Requests;

public sealed record CreateProductRequest(
    [Required(ErrorMessage = "Category identifier is required")]
    Guid CategoryId,
    [Required(ErrorMessage = "Manufacturer identifier is required")]
    Guid ManufacturerId,
    [Required(ErrorMessage = "Product model name is required")]
    [StringLength(255, MinimumLength = 1, ErrorMessage = "Model Name must be between 1 and 255 characters")]
    string ModelName,
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    string Description
);

public sealed record UpdateProductRequest(
    [Required]
    Guid Id,
    [Required(ErrorMessage = "Category identifier is required")]
    Guid CategoryId,
    [Required(ErrorMessage = "Manufacturer identifier is required")]
    Guid ManufacturerId,
    [Required(ErrorMessage = "Product model name is required")]
    [StringLength(255, MinimumLength = 1, ErrorMessage = "Model Name must be between 1 and 255 characters")]
    string ModelName,
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    string Description
);
