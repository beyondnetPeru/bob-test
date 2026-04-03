using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.Common.Models;
using Application.Features.AssetConfigurations.Queries.GetAssetConfigurationList;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Application.Features.AssetConfigurations.Queries.GetAssetConfigurationById;

public sealed record GetAssetConfigurationByIdQuery(Guid Id) : IQuery<AssetConfigurationDto>;

public sealed class GetAssetConfigurationByIdHandler(
    IAssetConfigurationRepository repository,
    IMapper mapper,
    ILogger<GetAssetConfigurationByIdHandler> logger)
    : IQueryHandler<GetAssetConfigurationByIdQuery, AssetConfigurationDto>
{
    public async Task<Result<AssetConfigurationDto>> Handle(GetAssetConfigurationByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Retrieving asset configuration: {Id}", request.Id);

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            logger.LogWarning("Asset configuration {Id} not found", request.Id);
            return Result.Failure<AssetConfigurationDto>(Error.NotFound(nameof(Domain.Entities.AssetConfiguration), request.Id));
        }

        return Result.Success(mapper.Map<AssetConfigurationDto>(entity));
    }
}
