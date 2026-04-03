using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.Common.Models;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Features.Inventory.Commands.UpdateHardware;

public sealed record UpdateHardwareCommand(Guid Id, string AssetName, decimal WeightKg) : ICommand;

public sealed class UpdateHardwareHandler(
    IInventoryRepository repository,
    IUnitOfWork unitOfWork,
    ILogger<UpdateHardwareHandler> logger)
    : ICommandHandler<UpdateHardwareCommand>
{
    public async Task<Result> Handle(UpdateHardwareCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating hardware asset: {Id}", request.Id);

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
            return Result.Failure(Error.NotFound(nameof(HardwareInventory), request.Id));

        entity.UpdateBasicSpecs(request.AssetName, request.WeightKg);
        repository.Update(entity);
        await unitOfWork.CommitAsync(cancellationToken);

        logger.LogInformation("Updated hardware asset {Id}", request.Id);
        return Result.Success();
    }
}
