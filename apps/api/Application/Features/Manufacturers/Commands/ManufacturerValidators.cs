using Application.Features.Manufacturers.Commands.CreateManufacturer;
using Application.Features.Manufacturers.Commands.DeleteManufacturer;
using Application.Features.Manufacturers.Commands.UpdateManufacturer;
using FluentValidation;

namespace Application.Features.Manufacturers.Commands;

public sealed class CreateManufacturerCommandValidator : AbstractValidator<CreateManufacturerCommand>
{
    public CreateManufacturerCommandValidator() =>
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
}

public sealed class UpdateManufacturerCommandValidator : AbstractValidator<UpdateManufacturerCommand>
{
    public UpdateManufacturerCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}

public sealed class DeleteManufacturerCommandValidator : AbstractValidator<DeleteManufacturerCommand>
{
    public DeleteManufacturerCommandValidator() =>
        RuleFor(x => x.Id).NotEmpty();
}
