using System.ComponentModel.DataAnnotations;

namespace Application.Features.Inventory.Commands.Requests;

public sealed record CreateHardwareRequest(
    [Required(ErrorMessage = "Asset name is required")]
    [StringLength(150, MinimumLength = 1, ErrorMessage = "Asset Name must be between 1 and 150 characters")]
    string AssetName,
    [Required]
    [Range(0.01, 10000.00, ErrorMessage = "Weight must be between 0.01 and 10000.00 kg")]
    decimal WeightKg
);

public sealed record UpdateHardwareRequest(
    [Required]
    Guid Id,
    [Required(ErrorMessage = "Asset name is required")]
    [StringLength(150, MinimumLength = 1, ErrorMessage = "Asset Name must be between 1 and 150 characters")]
    string AssetName,
    [Required]
    [Range(0.01, 10000.00, ErrorMessage = "Weight must be between 0.01 and 10000.00 kg")]
    decimal WeightKg
);
