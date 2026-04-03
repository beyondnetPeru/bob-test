using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.Common.Models;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.Commands.UpdateProduct;

public sealed record UpdateProductCommand(
    Guid Id,
    Guid CategoryId,
    Guid ManufacturerId,
    string ModelName,
    string Description) : ICommand;

public sealed class UpdateProductHandler(
    IProductRepository repository,
    ICategoryRepository categoryRepository,
    IManufacturerRepository manufacturerRepository,
    IUnitOfWork unitOfWork,
    ILogger<UpdateProductHandler> logger)
    : ICommandHandler<UpdateProductCommand>
{
    public async Task<Result> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating product: {Id}", request.Id);

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
            return Result.Failure(Error.NotFound(nameof(Product), request.Id));

        if (await categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken) is null)
            return Result.Failure(Error.NotFound(nameof(ProductCategory), request.CategoryId));

        if (await manufacturerRepository.GetByIdAsync(request.ManufacturerId, cancellationToken) is null)
            return Result.Failure(Error.NotFound(nameof(Manufacturer), request.ManufacturerId));

        entity.Update(request.CategoryId, request.ManufacturerId, request.ModelName, request.Description);
        repository.Update(entity);
        await unitOfWork.CommitAsync(cancellationToken);

        logger.LogInformation("Updated product {Id}", request.Id);
        return Result.Success();
    }
}
