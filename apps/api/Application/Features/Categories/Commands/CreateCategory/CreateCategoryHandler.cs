using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.Common.Models;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Features.Categories.Commands.CreateCategory;

public sealed record CreateCategoryCommand(string Name) : ICommand<Guid>;

public sealed class CreateCategoryHandler(
    ICategoryRepository repository,
    IUnitOfWork unitOfWork,
    ILogger<CreateCategoryHandler> logger)
    : ICommandHandler<CreateCategoryCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating category: {Name}", request.Name);

        var entity = new ProductCategory(request.Name);
        await repository.AddAsync(entity, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        logger.LogInformation("Created category {Name} ({Id})", request.Name, entity.Id);
        return Result.Success(entity.Id);
    }
}
