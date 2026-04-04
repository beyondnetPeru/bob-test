using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.Common.Models;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Application.Features.Inventory.Queries.GetInventoryList;

public sealed record GetInventoryListQuery : IQuery<List<InventoryListDto>>;

public sealed class InventoryListDto
{
    public Guid Id { get; init; }
    public string AssetName { get; init; } = string.Empty;
    public string DeviceCategory { get; init; } = "Desktop PC";
    public decimal WeightKg { get; init; }
    public string? PerformanceTier { get; init; }
    public int ComponentCount { get; init; }
    public string Cpu { get; init; } = "-";
    public string Ram { get; init; } = "-";
    public string HardDisc { get; init; } = "-";
    public string Ports { get; init; } = "-";
    public string Video { get; init; } = "-";
    public string PowerSource { get; init; } = "-";
}

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
