using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.Common.Models;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Application.Features.Manufacturers.Queries.GetManufacturerList;

public sealed record GetManufacturerListQuery : IQuery<List<ManufacturerDto>>;

public sealed record ManufacturerDto(Guid Id, string Name);

public sealed class GetManufacturerListHandler(
    IManufacturerRepository repository,
    IMapper mapper,
    ILogger<GetManufacturerListHandler> logger)
    : IQueryHandler<GetManufacturerListQuery, List<ManufacturerDto>>
{
    public async Task<Result<List<ManufacturerDto>>> Handle(GetManufacturerListQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Retrieving all manufacturers");
        var entities = await repository.GetAllAsync(cancellationToken);
        return Result.Success(mapper.Map<List<ManufacturerDto>>(entities));
    }
}
