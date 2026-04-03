using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.Common.Models;
using Microsoft.Extensions.Logging;

namespace Application.Features.Categories.Commands.UpdateCategory;

public sealed record UpdateCategoryCommand(Guid Id, string Name) : ICommand;

public sealed class UpdateCategoryHandler(
    ICategoryRepository repository,
    IUnitOfWork unitOfWork,
    ILogger<UpdateCategoryHandler> logger)
    : ICommandHandler<UpdateCategoryCommand>
{
    public async Task<Result> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating category: {Id}", request.Id);

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            logger.LogWarning("Category {Id} not found", request.Id);
            return Result.Failure(Error.NotFound(nameof(Domain.Entities.ProductCategory), request.Id));
        }

        entity.Update(request.Name);
        repository.Update(entity);
        await unitOfWork.CommitAsync(cancellationToken);

        logger.LogInformation("Updated category {Id}", request.Id);
        return Result.Success();
    }
}
