using System.ComponentModel.DataAnnotations;

namespace Application.Features.Manufacturers.Commands.Requests;

public sealed record CreateManufacturerRequest(
    [Required(ErrorMessage = "Manufacturer name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 100 characters")]
    string Name);

public sealed record UpdateManufacturerRequest(
    [Required]
    Guid Id,
    [Required(ErrorMessage = "Manufacturer name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 100 characters")]
    string Name);
