using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.Common.Models;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Features.AssetConfigurations.Commands.UpdateAssetConfiguration;

public sealed record UpdateAssetConfigurationCommand(
    Guid Id,
    Guid InventoryId,
    Guid ProductId,
    int Quantity,
    decimal? StandardValue,
    string? Location) : ICommand;

public sealed class UpdateAssetConfigurationHandler(
    IAssetConfigurationRepository repository,
    IInventoryRepository inventoryRepository,
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    ILogger<UpdateAssetConfigurationHandler> logger)
    : ICommandHandler<UpdateAssetConfigurationCommand>
{
    public async Task<Result> Handle(UpdateAssetConfigurationCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating asset configuration: {Id}", request.Id);

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
            return Result.Failure(Error.NotFound(nameof(AssetConfiguration), request.Id));

        if (await inventoryRepository.GetByIdAsync(request.InventoryId, cancellationToken) is null)
            return Result.Failure(Error.NotFound(nameof(HardwareInventory), request.InventoryId));

        if (await productRepository.GetByIdAsync(request.ProductId, cancellationToken) is null)
            return Result.Failure(Error.NotFound(nameof(Product), request.ProductId));

        entity.Update(request.InventoryId, request.ProductId, request.Quantity, request.StandardValue, request.Location);
        repository.Update(entity);
        await unitOfWork.CommitAsync(cancellationToken);

        logger.LogInformation("Updated asset configuration {Id}", request.Id);
        return Result.Success();
    }
}
