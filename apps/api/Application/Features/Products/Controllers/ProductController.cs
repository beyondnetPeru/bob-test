using Application.Abstractions.Controllers;
using Application.Features.Products.Commands.CreateProduct;
using Application.Features.Products.Commands.DeleteProduct;
using Application.Features.Products.Commands.Requests;
using Application.Features.Products.Commands.UpdateProduct;
using Application.Features.Products.Queries.GetProductById;
using Application.Features.Products.Queries.GetProductList;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public sealed class ProductController(IMediator mediator) : ApiControllerBase(mediator)
{
    [HttpGet]
    [ProducesResponseType(typeof(List<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ProductDto>>> GetProducts()
        => HandleQuery(await Mediator.Send(new GetProductListQuery()));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetProduct(Guid id)
        => HandleQuery(await Mediator.Send(new GetProductByIdQuery(id)));

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Guid>> CreateProduct(CreateProductRequest request)
        => HandleCreate(
            await Mediator.Send(new CreateProductCommand(
                request.CategoryId,
                request.ManufacturerId,
                request.ModelName,
                request.Description)),
            nameof(GetProduct));

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProduct(Guid id, UpdateProductRequest request)
    {
        var mismatch = ValidateIdMatch(id, request.Id);
        if (mismatch is not null) return mismatch;

        return HandleCommand(await Mediator.Send(new UpdateProductCommand(
            request.Id,
            request.CategoryId,
            request.ManufacturerId,
            request.ModelName,
            request.Description)));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(Guid id)
        => HandleCommand(await Mediator.Send(new DeleteProductCommand(id)));
}
