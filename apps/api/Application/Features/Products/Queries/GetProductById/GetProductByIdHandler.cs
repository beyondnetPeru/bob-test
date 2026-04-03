using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.Common.Models;
using Application.Features.Products.Queries.GetProductList;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.Queries.GetProductById;

public sealed record GetProductByIdQuery(Guid Id) : IQuery<ProductDto>;

public sealed class GetProductByIdHandler(
    IProductRepository repository,
    IMapper mapper,
    ILogger<GetProductByIdHandler> logger)
    : IQueryHandler<GetProductByIdQuery, ProductDto>
{
    public async Task<Result<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Retrieving product: {Id}", request.Id);

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            logger.LogWarning("Product {Id} not found", request.Id);
            return Result.Failure<ProductDto>(Error.NotFound(nameof(Domain.Entities.Product), request.Id));
        }

        return Result.Success(mapper.Map<ProductDto>(entity));
    }
}
