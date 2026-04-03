using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.Common.Models;
using Application.Features.Inventory.Queries.GetInventoryList;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Application.Features.Inventory.Queries.GetHardwareById;

public sealed record GetHardwareByIdQuery(Guid Id) : IQuery<InventoryListDto>;

public sealed class GetHardwareByIdHandler(
    IInventoryRepository repository,
    IMapper mapper,
    ILogger<GetHardwareByIdHandler> logger)
    : IQueryHandler<GetHardwareByIdQuery, InventoryListDto>
{
    public async Task<Result<InventoryListDto>> Handle(GetHardwareByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Retrieving hardware asset: {Id}", request.Id);

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            logger.LogWarning("Hardware asset {Id} not found", request.Id);
            return Result.Failure<InventoryListDto>(Error.NotFound(nameof(Domain.Entities.HardwareInventory), request.Id));
        }

        return Result.Success(mapper.Map<InventoryListDto>(entity));
    }
}
