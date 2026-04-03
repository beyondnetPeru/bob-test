using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.Common.Models;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Features.Manufacturers.Commands.CreateManufacturer;

public sealed record CreateManufacturerCommand(string Name) : ICommand<Guid>;

public sealed class CreateManufacturerHandler(
    IManufacturerRepository repository,
    IUnitOfWork unitOfWork,
    ILogger<CreateManufacturerHandler> logger)
    : ICommandHandler<CreateManufacturerCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateManufacturerCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating manufacturer: {Name}", request.Name);

        var entity = new Manufacturer(request.Name);
        await repository.AddAsync(entity, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        logger.LogInformation("Created manufacturer {Name} ({Id})", request.Name, entity.Id);
        return Result.Success(entity.Id);
    }
}
