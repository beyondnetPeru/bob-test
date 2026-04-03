using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.Common.Models;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.Commands.DeleteProduct;

public sealed record DeleteProductCommand(Guid Id) : ICommand;

public sealed class DeleteProductHandler(
    IProductRepository repository,
    IUnitOfWork unitOfWork,
    ILogger<DeleteProductHandler> logger)
    : ICommandHandler<DeleteProductCommand>
{
    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting product: {Id}", request.Id);

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
            return Result.Failure(Error.NotFound(nameof(Product), request.Id));

        repository.Delete(entity);
        await unitOfWork.CommitAsync(cancellationToken);

        logger.LogInformation("Deleted product {Id}", request.Id);
        return Result.Success();
    }
}
