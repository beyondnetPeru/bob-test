using Application.Features.AssetConfigurations.Commands.CreateAssetConfiguration;
using Application.Features.AssetConfigurations.Commands.DeleteAssetConfiguration;
using Application.Features.AssetConfigurations.Commands.UpdateAssetConfiguration;
using FluentValidation;

namespace Application.Features.AssetConfigurations.Commands;

public sealed class CreateAssetConfigurationCommandValidator : AbstractValidator<CreateAssetConfigurationCommand>
{
    public CreateAssetConfigurationCommandValidator()
    {
        RuleFor(x => x.InventoryId).NotEmpty();
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}

public sealed class UpdateAssetConfigurationCommandValidator : AbstractValidator<UpdateAssetConfigurationCommand>
{
    public UpdateAssetConfigurationCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.InventoryId).NotEmpty();
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}

public sealed class DeleteAssetConfigurationCommandValidator : AbstractValidator<DeleteAssetConfigurationCommand>
{
    public DeleteAssetConfigurationCommandValidator() =>
        RuleFor(x => x.Id).NotEmpty();
}
