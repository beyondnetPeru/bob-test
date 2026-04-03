using Application.Abstractions.Controllers;
using Application.Features.AssetConfigurations.Commands.CreateAssetConfiguration;
using Application.Features.AssetConfigurations.Commands.DeleteAssetConfiguration;
using Application.Features.AssetConfigurations.Commands.Requests;
using Application.Features.AssetConfigurations.Commands.UpdateAssetConfiguration;
using Application.Features.AssetConfigurations.Queries.GetAssetConfigurationById;
using Application.Features.AssetConfigurations.Queries.GetAssetConfigurationList;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public sealed class AssetConfigurationController(IMediator mediator) : ApiControllerBase(mediator)
{
    [HttpGet]
    [ProducesResponseType(typeof(List<AssetConfigurationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<AssetConfigurationDto>>> GetAssetConfigurations()
        => HandleQuery(await Mediator.Send(new GetAssetConfigurationListQuery()));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AssetConfigurationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AssetConfigurationDto>> GetAssetConfiguration(Guid id)
        => HandleQuery(await Mediator.Send(new GetAssetConfigurationByIdQuery(id)));

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Guid>> CreateAssetConfiguration(CreateAssetConfigurationRequest request)
        => HandleCreate(
            await Mediator.Send(new CreateAssetConfigurationCommand(
                request.InventoryId,
                request.ProductId,
                request.Quantity,
                request.StandardValue,
                request.Location)),
            nameof(GetAssetConfiguration));

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAssetConfiguration(Guid id, UpdateAssetConfigurationRequest request)
    {
        var mismatch = ValidateIdMatch(id, request.Id);
        if (mismatch is not null) return mismatch;

        return HandleCommand(await Mediator.Send(new UpdateAssetConfigurationCommand(
            request.Id,
            request.InventoryId,
            request.ProductId,
            request.Quantity,
            request.StandardValue,
            request.Location)));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAssetConfiguration(Guid id)
        => HandleCommand(await Mediator.Send(new DeleteAssetConfigurationCommand(id)));
}
