using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.Common.Models;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Application.Features.Categories.Queries.GetCategoryList;

public sealed record GetCategoryListQuery : IQuery<List<CategoryDto>>;

public sealed record CategoryDto(Guid Id, string Name);

public sealed class GetCategoryListHandler(
    ICategoryRepository repository,
    IMapper mapper,
    ILogger<GetCategoryListHandler> logger)
    : IQueryHandler<GetCategoryListQuery, List<CategoryDto>>
{
    public async Task<Result<List<CategoryDto>>> Handle(GetCategoryListQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Retrieving all categories");
        var entities = await repository.GetAllAsync(cancellationToken);
        return Result.Success(mapper.Map<List<CategoryDto>>(entities));
    }
}
