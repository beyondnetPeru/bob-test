using Application.Abstractions.Controllers;
using Application.Features.Inventory.Commands.CreateHardware;
using Application.Features.Inventory.Commands.DeleteHardware;
using Application.Features.Inventory.Commands.Requests;
using Application.Features.Inventory.Commands.UpdateHardware;
using Application.Features.Inventory.Queries.GetHardwareById;
using Application.Features.Inventory.Queries.GetInventoryList;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public sealed class InventoryController(IMediator mediator) : ApiControllerBase(mediator)
{
    [HttpGet]
    [ProducesResponseType(typeof(List<InventoryListDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<InventoryListDto>>> GetInventory()
        => HandleQuery(await Mediator.Send(new GetInventoryListQuery()));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(InventoryListDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InventoryListDto>> GetInventoryItem(Guid id)
        => HandleQuery(await Mediator.Send(new GetHardwareByIdQuery(id)));

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> CreateHardware(CreateHardwareRequest request)
        => HandleCreate(
            await Mediator.Send(new CreateHardwareCommand(request.AssetName, request.WeightKg)),
            nameof(GetInventoryItem));

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateHardware(Guid id, UpdateHardwareRequest request)
    {
        var mismatch = ValidateIdMatch(id, request.Id);
        if (mismatch is not null) return mismatch;

        return HandleCommand(await Mediator.Send(new UpdateHardwareCommand(request.Id, request.AssetName, request.WeightKg)));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteHardware(Guid id)
        => HandleCommand(await Mediator.Send(new DeleteHardwareCommand(id)));
}
