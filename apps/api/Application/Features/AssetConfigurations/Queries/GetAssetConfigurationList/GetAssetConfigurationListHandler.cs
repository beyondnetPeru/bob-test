using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.Common.Models;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Application.Features.AssetConfigurations.Queries.GetAssetConfigurationList;

public sealed record GetAssetConfigurationListQuery : IQuery<List<AssetConfigurationDto>>;

public sealed class AssetConfigurationDto
{
    public Guid Id { get; init; }
    public Guid InventoryId { get; init; }
    public string InventoryName { get; init; } = string.Empty;
    public Guid ProductId { get; init; }
    public string ProductModelName { get; init; } = string.Empty;
    public string CategoryName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal? StandardValue { get; init; }
    public string? Location { get; init; }
}

public sealed class GetAssetConfigurationListHandler(
    IAssetConfigurationRepository repository,
    IMapper mapper,
    ILogger<GetAssetConfigurationListHandler> logger)
    : IQueryHandler<GetAssetConfigurationListQuery, List<AssetConfigurationDto>>
{
    public async Task<Result<List<AssetConfigurationDto>>> Handle(GetAssetConfigurationListQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Retrieving all asset configurations");
        var entities = await repository.GetAllAsync(cancellationToken);
        return Result.Success(mapper.Map<List<AssetConfigurationDto>>(entities));
    }
}
