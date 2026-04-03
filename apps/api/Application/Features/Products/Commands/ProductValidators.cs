using Application.Features.Products.Commands.CreateProduct;
using Application.Features.Products.Commands.DeleteProduct;
using Application.Features.Products.Commands.UpdateProduct;
using FluentValidation;

namespace Application.Features.Products.Commands;

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.ModelName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.ManufacturerId).NotEmpty();
    }
}

public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.ModelName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.ManufacturerId).NotEmpty();
    }
}

public sealed class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator() =>
        RuleFor(x => x.Id).NotEmpty();
}
