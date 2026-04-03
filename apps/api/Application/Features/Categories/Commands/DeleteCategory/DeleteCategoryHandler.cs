using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.Common.Models;
using Microsoft.Extensions.Logging;

namespace Application.Features.Categories.Commands.DeleteCategory;

public sealed record DeleteCategoryCommand(Guid Id) : ICommand;

public sealed class DeleteCategoryHandler(
    ICategoryRepository repository,
    IUnitOfWork unitOfWork,
    ILogger<DeleteCategoryHandler> logger)
    : ICommandHandler<DeleteCategoryCommand>
{
    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting category: {Id}", request.Id);

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            logger.LogWarning("Category {Id} not found", request.Id);
            return Result.Failure(Error.NotFound(nameof(Domain.Entities.ProductCategory), request.Id));
        }

        repository.Delete(entity);
        await unitOfWork.CommitAsync(cancellationToken);

        logger.LogInformation("Deleted category {Id}", request.Id);
        return Result.Success();
    }
}
