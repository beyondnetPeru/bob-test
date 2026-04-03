using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.Common.Models;
using Microsoft.Extensions.Logging;

namespace Application.Features.Manufacturers.Commands.DeleteManufacturer;

public sealed record DeleteManufacturerCommand(Guid Id) : ICommand;

public sealed class DeleteManufacturerHandler(
    IManufacturerRepository repository,
    IUnitOfWork unitOfWork,
    ILogger<DeleteManufacturerHandler> logger)
    : ICommandHandler<DeleteManufacturerCommand>
{
    public async Task<Result> Handle(DeleteManufacturerCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting manufacturer: {Id}", request.Id);

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            logger.LogWarning("Manufacturer {Id} not found", request.Id);
            return Result.Failure(Error.NotFound(nameof(Domain.Entities.Manufacturer), request.Id));
        }

        repository.Delete(entity);
        await unitOfWork.CommitAsync(cancellationToken);

        logger.LogInformation("Deleted manufacturer {Id}", request.Id);
        return Result.Success();
    }
}
