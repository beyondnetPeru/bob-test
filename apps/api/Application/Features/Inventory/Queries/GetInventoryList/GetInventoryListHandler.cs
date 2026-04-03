using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.Common.Models;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Application.Features.Inventory.Queries.GetInventoryList;

public sealed record GetInventoryListQuery : IQuery<List<InventoryListDto>>;

public sealed record InventoryListDto(
    Guid Id,
    string AssetName,
    decimal WeightKg,
    string? PerformanceTier,
    int ComponentCount);

public sealed class GetInventoryListHandler(
    IInventoryRepository repository,
    IMapper mapper,
    ILogger<GetInventoryListHandler> logger)
    : IQueryHandler<GetInventoryListQuery, List<InventoryListDto>>
{
    public async Task<Result<List<InventoryListDto>>> Handle(GetInventoryListQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Retrieving all hardware inventory assets");
        var entities = await repository.GetAllAsync(cancellationToken);
        return Result.Success(mapper.Map<List<InventoryListDto>>(entities));
    }
}
