using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.Common.Models;
using Application.Features.Manufacturers.Queries.GetManufacturerList;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Application.Features.Manufacturers.Queries.GetManufacturerById;

public sealed record GetManufacturerByIdQuery(Guid Id) : IQuery<ManufacturerDto>;

public sealed class GetManufacturerByIdHandler(
    IManufacturerRepository repository,
    IMapper mapper,
    ILogger<GetManufacturerByIdHandler> logger)
    : IQueryHandler<GetManufacturerByIdQuery, ManufacturerDto>
{
    public async Task<Result<ManufacturerDto>> Handle(GetManufacturerByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Retrieving manufacturer: {Id}", request.Id);

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            logger.LogWarning("Manufacturer {Id} not found", request.Id);
            return Result.Failure<ManufacturerDto>(Error.NotFound(nameof(Domain.Entities.Manufacturer), request.Id));
        }

        return Result.Success(mapper.Map<ManufacturerDto>(entity));
    }
}
