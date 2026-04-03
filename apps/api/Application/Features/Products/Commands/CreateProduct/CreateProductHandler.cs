using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.Common.Models;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.Commands.CreateProduct;

public sealed record CreateProductCommand(
    Guid CategoryId,
    Guid ManufacturerId,
    string ModelName,
    string Description) : ICommand<Guid>;

public sealed class CreateProductHandler(
    IProductRepository repository,
    ICategoryRepository categoryRepository,
    IManufacturerRepository manufacturerRepository,
    IUnitOfWork unitOfWork,
    ILogger<CreateProductHandler> logger)
    : ICommandHandler<CreateProductCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating product: {ModelName}", request.ModelName);

        if (await categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken) is null)
            return Result.Failure<Guid>(Error.NotFound(nameof(ProductCategory), request.CategoryId));

        if (await manufacturerRepository.GetByIdAsync(request.ManufacturerId, cancellationToken) is null)
            return Result.Failure<Guid>(Error.NotFound(nameof(Manufacturer), request.ManufacturerId));

        var entity = new Product(request.CategoryId, request.ManufacturerId, request.ModelName, request.Description);
        await repository.AddAsync(entity, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        logger.LogInformation("Created product {ModelName} ({Id})", request.ModelName, entity.Id);
        return Result.Success(entity.Id);
    }
}
