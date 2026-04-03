using Application.Abstractions.Controllers;
using Application.Features.Manufacturers.Commands.CreateManufacturer;
using Application.Features.Manufacturers.Commands.DeleteManufacturer;
using Application.Features.Manufacturers.Commands.Requests;
using Application.Features.Manufacturers.Commands.UpdateManufacturer;
using Application.Features.Manufacturers.Queries.GetManufacturerById;
using Application.Features.Manufacturers.Queries.GetManufacturerList;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public sealed class ManufacturerController(IMediator mediator) : ApiControllerBase(mediator)
{
    [HttpGet]
    [ProducesResponseType(typeof(List<ManufacturerDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ManufacturerDto>>> GetManufacturers()
        => HandleQuery(await Mediator.Send(new GetManufacturerListQuery()));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ManufacturerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ManufacturerDto>> GetManufacturer(Guid id)
        => HandleQuery(await Mediator.Send(new GetManufacturerByIdQuery(id)));

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> CreateManufacturer(CreateManufacturerRequest request)
        => HandleCreate(
            await Mediator.Send(new CreateManufacturerCommand(request.Name)),
            nameof(GetManufacturer));

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateManufacturer(Guid id, UpdateManufacturerRequest request)
    {
        var mismatch = ValidateIdMatch(id, request.Id);
        if (mismatch is not null) return mismatch;

        return HandleCommand(await Mediator.Send(new UpdateManufacturerCommand(request.Id, request.Name)));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteManufacturer(Guid id)
        => HandleCommand(await Mediator.Send(new DeleteManufacturerCommand(id)));
}
