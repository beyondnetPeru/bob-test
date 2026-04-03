using System.ComponentModel.DataAnnotations;

namespace Application.Features.Categories.Commands.Requests;

public sealed record CreateCategoryRequest(
    [Required(ErrorMessage = "Category name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 100 characters")]
    string Name);

public sealed record UpdateCategoryRequest(
    [Required]
    Guid Id,
    [Required(ErrorMessage = "Category name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 100 characters")]
    string Name);
