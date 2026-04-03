using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.Common.Models;
using Application.Features.Products.Queries.GetProductList;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.Queries.GetProductList;

public sealed record GetProductListQuery : IQuery<List<ProductDto>>;

public sealed record ProductDto(
    Guid Id,
    string ModelName,
    string? Description,
    Guid CategoryId,
    string? CategoryName,
    Guid ManufacturerId,
    string? ManufacturerName);

public sealed class GetProductListHandler(
    IProductRepository repository,
    IMapper mapper,
    ILogger<GetProductListHandler> logger)
    : IQueryHandler<GetProductListQuery, List<ProductDto>>
{
    public async Task<Result<List<ProductDto>>> Handle(GetProductListQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Retrieving all products");
        var entities = await repository.GetAllAsync(cancellationToken);
        return Result.Success(mapper.Map<List<ProductDto>>(entities));
    }
}
