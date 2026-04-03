using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.Common.Models;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Features.Inventory.Commands.DeleteHardware;

public sealed record DeleteHardwareCommand(Guid Id) : ICommand;

public sealed class DeleteHardwareHandler(
    IInventoryRepository repository,
    IUnitOfWork unitOfWork,
    ILogger<DeleteHardwareHandler> logger)
    : ICommandHandler<DeleteHardwareCommand>
{
    public async Task<Result> Handle(DeleteHardwareCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting hardware asset: {Id}", request.Id);

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
            return Result.Failure(Error.NotFound(nameof(HardwareInventory), request.Id));

        repository.Delete(entity);
        await unitOfWork.CommitAsync(cancellationToken);

        logger.LogInformation("Deleted hardware asset {Id}", request.Id);
        return Result.Success();
    }
}
