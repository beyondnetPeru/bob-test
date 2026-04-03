using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.Common.Models;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Features.AssetConfigurations.Commands.CreateAssetConfiguration;

public sealed record CreateAssetConfigurationCommand(
    Guid InventoryId,
    Guid ProductId,
    int Quantity,
    decimal? StandardValue,
    string? Location) : ICommand<Guid>;

public sealed class CreateAssetConfigurationHandler(
    IAssetConfigurationRepository repository,
    IInventoryRepository inventoryRepository,
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    ILogger<CreateAssetConfigurationHandler> logger)
    : ICommandHandler<CreateAssetConfigurationCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateAssetConfigurationCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating asset configuration for inventory {InventoryId}", request.InventoryId);

        if (await inventoryRepository.GetByIdAsync(request.InventoryId, cancellationToken) is null)
            return Result.Failure<Guid>(Error.NotFound(nameof(HardwareInventory), request.InventoryId));

        if (await productRepository.GetByIdAsync(request.ProductId, cancellationToken) is null)
            return Result.Failure<Guid>(Error.NotFound(nameof(Product), request.ProductId));

        var entity = new AssetConfiguration(
            request.InventoryId,
            request.ProductId,
            request.Quantity,
            request.StandardValue,
            request.Location);

        await repository.AddAsync(entity, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        logger.LogInformation("Created asset configuration {Id}", entity.Id);
        return Result.Success(entity.Id);
    }
}
