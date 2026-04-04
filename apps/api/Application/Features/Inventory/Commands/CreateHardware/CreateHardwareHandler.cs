using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.Common.Models;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Features.Inventory.Commands.CreateHardware;

public sealed record CreateHardwareCommand(string AssetName, decimal WeightKg, string DeviceCategory = "Desktop PC") : ICommand<Guid>;

public sealed class CreateHardwareHandler(
    IInventoryRepository repository,
    IUnitOfWork unitOfWork,
    ILogger<CreateHardwareHandler> logger)
    : ICommandHandler<CreateHardwareCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateHardwareCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating hardware asset: {AssetName}", request.AssetName);

        var entity = new HardwareInventory(request.AssetName, request.WeightKg, request.DeviceCategory);
        await repository.AddAsync(entity, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        logger.LogInformation("Created hardware asset {AssetName} ({Id})", request.AssetName, entity.Id);
        return Result.Success(entity.Id);
    }
}
