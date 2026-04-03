using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.Common.Models;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Features.AssetConfigurations.Commands.DeleteAssetConfiguration;

public sealed record DeleteAssetConfigurationCommand(Guid Id) : ICommand;

public sealed class DeleteAssetConfigurationHandler(
    IAssetConfigurationRepository repository,
    IUnitOfWork unitOfWork,
    ILogger<DeleteAssetConfigurationHandler> logger)
    : ICommandHandler<DeleteAssetConfigurationCommand>
{
    public async Task<Result> Handle(DeleteAssetConfigurationCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting asset configuration: {Id}", request.Id);

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
            return Result.Failure(Error.NotFound(nameof(AssetConfiguration), request.Id));

        repository.Delete(entity);
        await unitOfWork.CommitAsync(cancellationToken);

        logger.LogInformation("Deleted asset configuration {Id}", request.Id);
        return Result.Success();
    }
}
