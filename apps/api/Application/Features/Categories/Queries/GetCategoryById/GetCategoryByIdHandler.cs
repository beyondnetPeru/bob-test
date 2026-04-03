using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.Common.Models;
using Application.Features.Categories.Queries.GetCategoryList;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Application.Features.Categories.Queries.GetCategoryById;

public sealed record GetCategoryByIdQuery(Guid Id) : IQuery<CategoryDto>;

public sealed class GetCategoryByIdHandler(
    ICategoryRepository repository,
    IMapper mapper,
    ILogger<GetCategoryByIdHandler> logger)
    : IQueryHandler<GetCategoryByIdQuery, CategoryDto>
{
    public async Task<Result<CategoryDto>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Retrieving category: {Id}", request.Id);

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            logger.LogWarning("Category {Id} not found", request.Id);
            return Result.Failure<CategoryDto>(Error.NotFound(nameof(Domain.Entities.ProductCategory), request.Id));
        }

        return Result.Success(mapper.Map<CategoryDto>(entity));
    }
}
