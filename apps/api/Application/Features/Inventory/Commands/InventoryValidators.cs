using Application.Features.Inventory.Commands.CreateHardware;
using Application.Features.Inventory.Commands.DeleteHardware;
using Application.Features.Inventory.Commands.UpdateHardware;
using FluentValidation;

namespace Application.Features.Inventory.Commands;

public sealed class CreateHardwareCommandValidator : AbstractValidator<CreateHardwareCommand>
{
    public CreateHardwareCommandValidator()
    {
        RuleFor(x => x.AssetName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.WeightKg).GreaterThan(0);
    }
}

public sealed class UpdateHardwareCommandValidator : AbstractValidator<UpdateHardwareCommand>
{
    public UpdateHardwareCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.AssetName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.WeightKg).GreaterThan(0);
    }
}

public sealed class DeleteHardwareCommandValidator : AbstractValidator<DeleteHardwareCommand>
{
    public DeleteHardwareCommandValidator() =>
        RuleFor(x => x.Id).NotEmpty();
}
